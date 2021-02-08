using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupRepositories.Services;
using BackupRepositories.Services.RepositoryReferences;
using Microsoft.Extensions.Logging;

namespace BackupRepositories.Handlers
{
    public class BackupRepositoriesHandler : IExecutionHandler
    {
        private readonly ILogger<BackupRepositoriesHandler> logger;
        private readonly IRemoteRepositoryService remoteRepositoryService;
        private readonly LocalRepositoryService localRepositoryService;
        private readonly GitService gitService;
        private readonly IReportService reportService;

        public BackupRepositoriesHandler(
            ILogger<BackupRepositoriesHandler> logger,
            IRemoteRepositoryService remoteRepositoryService,
            LocalRepositoryService localRepositoryService,
            GitService gitService,
            IReportService reportService)
        {
            this.logger = logger;
            this.remoteRepositoryService = remoteRepositoryService;
            this.localRepositoryService = localRepositoryService;
            this.gitService = gitService;
            this.reportService = reportService;
        }

        public async Task Execute(string[] args)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                logger.LogInformation("Requesting all remote repository references...");

                var referencesResult = await remoteRepositoryService.GetAllRemoteRepositoryReferences();
                referencesResult.Match(
                    references =>
                    {
                        // Backup
                        BackupRemoteRepositories(references);
                        // Delete 
                        DeleteRemovedRemoteRepositories(references);
                    }, error =>
                    {
                        logger.LogError(error);
                        reportService.AddReportForError(error);
                    });

                watch.Stop();
                reportService.AddReportForElapsedTimeOfBackupInMilliseconds(watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e}");
                reportService.AddReportForError($"Error: {e.Message}");
            }


            logger.LogInformation("Pushing results to report server...");
            var publish = await reportService.Publish();
            publish.Match(
                success => logger.LogDebug(success),
                error => logger.LogError(error));
        }

        private void BackupRemoteRepositories(List<RemoteRepositoryReference> remoteRepositoryReferences)
        {
            logger.LogInformation("Starting backup process for all received remote repositories");
            LogReceivedRemoteRepositories(remoteRepositoryReferences);

            reportService.AddReportForRemoteRepositories(remoteRepositoryReferences);
            remoteRepositoryReferences.ForEach(remoteRepositoryReference =>
            {
                logger.LogInformation(
                    $"Starting backup process for repository: {remoteRepositoryReference.RepositoryName}");

                var result = localRepositoryService.RepositoryDirectoryExists(remoteRepositoryReference)
                    ? gitService.FetchFromRemote(remoteRepositoryReference)
                    : gitService.CloneMirror(remoteRepositoryReference);
                logger.LogInformation(
                    $"Finished backup process for repository: {remoteRepositoryReference.RepositoryName}");

                result.Match(success =>
                {
                    logger.LogInformation(success.CreateLog());
                    reportService.AddReportForGitServiceReport(success);
                }, error =>
                {
                    logger.LogError(error);
                    reportService.AddReportForError(error);
                });
            });
        }

        private void DeleteRemovedRemoteRepositories(List<RemoteRepositoryReference> remoteRepositoryReferences)
        {
            reportService.AddReportForLocalRepositoriesBeforeDelete(localRepositoryService
                .GetAllLocalRepositoryReferences());
            var localRepositoryReferencesToDelete =
                localRepositoryService.GetLocalRepositoryReferencesToDelete(remoteRepositoryReferences);
            LogRepositoriesToDelete(localRepositoryReferencesToDelete);
            localRepositoryService.DeleteLocalRepositories(localRepositoryReferencesToDelete);
            reportService.AddReportForDeletedLocalRepositories(localRepositoryReferencesToDelete);
            reportService.AddReportForLocalRepositories(localRepositoryService.GetAllLocalRepositoryReferences());
        }

        private void LogRepositoriesToDelete(List<LocalRepositoryReference> localRepositoryReferencesToDelete)
        {
            var deleteMessage = $"Local Repositories to Delete:{Environment.NewLine}" + string.Join(
                Environment.NewLine,
                localRepositoryReferencesToDelete.Select(reference =>
                    $"Project: {reference.ProjectName} Repository: {reference.RepositoryName}"));
            logger.LogWarning(deleteMessage);
        }

        private void LogReceivedRemoteRepositories(List<RemoteRepositoryReference> remoteRepositoryReferences)
        {
            logger.LogInformation($"Remote repositories:{Environment.NewLine}" + string.Join(
                Environment.NewLine,
                remoteRepositoryReferences.Select(repo =>
                        $"RepositoryName: {repo.RepositoryName} Url: {repo.RemoteUrl}")
                    .ToArray()));
        }
    }
}