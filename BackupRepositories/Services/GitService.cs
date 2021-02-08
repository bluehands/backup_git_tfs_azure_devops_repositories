using System;
using System.Collections.Generic;
using System.Linq;
using BackupRepositories.Services.RepositoryReferences;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace BackupRepositories.Services
{
    public class GitService
    {
        private readonly ILogger<GitService> logger;
        private readonly IDataProtector tokeProtector;
        private readonly LocalRepositoryService localRepositoryService;
        private readonly GitServiceSettings serviceSettings;

        public GitService(
            ILogger<GitService> logger,
            IDataProtector tokeProtector,
            LocalRepositoryService localRepositoryService, 
            GitServiceSettings serviceSettings)
        {
            this.logger = logger;
            this.tokeProtector = tokeProtector;
            this.localRepositoryService = localRepositoryService;
            this.serviceSettings = serviceSettings;
        }
        public Result<GitServiceReport> CloneMirror(RemoteRepositoryReference remoteRepositoryReference)
        {
            try
            {
                return localRepositoryService
                    .CreateBareGitLibRepository(remoteRepositoryReference)
                    .Bind(repository =>
                        {
                            logger.LogDebug($"Created new bare repo for: {remoteRepositoryReference.RepositoryName}");
                            AddRepositoryRemote(remoteRepositoryReference, repository);
                            var gitServiceReport = CreateGitServiceReport(ReportType.CloneMirror, remoteRepositoryReference);
                            logger.LogInformation($"Starting git fetch of repository {remoteRepositoryReference.RepositoryName} for CloneMirror operation...");
                            Fetch(remoteRepositoryReference, repository, gitServiceReport);
                            return AssertFetchSuccess(remoteRepositoryReference, gitServiceReport);
                        }
                    );
            }
            catch (Exception e)
            {
                logger.LogError($"Git Error: {e}");
                return Result<GitServiceReport>.Error($"Git Error: {e.Message}");
            }
        }

        public Result<GitServiceReport> FetchFromRemote(RemoteRepositoryReference remoteRepositoryReference)
        {
            try
            {
                return localRepositoryService.GetGitLibRepository(remoteRepositoryReference).Bind(repository =>
                    {
                        logger.LogDebug($"Loaded repo for: {remoteRepositoryReference.RepositoryName}");
                        var gitServiceReport = CreateGitServiceReport(ReportType.FetchFromRemote, remoteRepositoryReference);
                        logger.LogInformation($"Starting git fetch of repository {remoteRepositoryReference.RepositoryName} for FetchFromRemote operation...");
                        Fetch(remoteRepositoryReference, repository, gitServiceReport);
                        return AssertFetchSuccess(remoteRepositoryReference, gitServiceReport);
                    });
            }
            catch (Exception e)
            {
                logger.LogError($"Git Error: {e}");
                return Result<GitServiceReport>.Error($"Git Error: {e.Message}");
            }
        }

        private static Result<GitServiceReport> AssertFetchSuccess(RemoteRepositoryReference remoteRepositoryReference,
            GitServiceReport gitServiceReport)
        {
            return gitServiceReport.IndexedObjects == gitServiceReport.TotalObjects
                ? Result.Ok(gitServiceReport)
                : Result<GitServiceReport>.Error(
                    $"Error during clone process! Failed indexing all files of repository: {remoteRepositoryReference.RepositoryName}");
        }

        private GitServiceReport CreateGitServiceReport(ReportType reportType, RemoteRepositoryReference remoteRepositoryReference)
        {
            return new GitServiceReport(
                reportType,
                localRepositoryService.GetPathToLocalRepository(remoteRepositoryReference),
                remoteRepositoryReference);
        }

        private void Fetch(RemoteRepositoryReference remoteRepositoryReference, Repository repository,
            GitServiceReport gitServiceReport)
        {
            Commands.Fetch(
                repository,
                GetRepositoryRemote(repository).Name,
                GetRefSpecs(GetRepositoryRemote(repository)),
                CreateFetchOptions(remoteRepositoryReference, gitServiceReport),
                string.Empty);
        }

        private void AddRepositoryRemote(RemoteRepositoryReference remoteRepositoryReference, IRepository gitLibRepository)
        {
            gitLibRepository.Network.Remotes.Add(serviceSettings.RemoteOriginName, remoteRepositoryReference.RemoteUrl, serviceSettings.FetchRefSpecs);
        }

        private Remote GetRepositoryRemote(IRepository gitLibRepository)
        {
            return gitLibRepository.Network.Remotes[serviceSettings.RemoteOriginName];
        }

        private static IEnumerable<string> GetRefSpecs(Remote gitLibRemote)
        {
            return gitLibRemote.FetchRefSpecs.Select(x => x.Specification);
        }

        private FetchOptions CreateFetchOptions(RemoteRepositoryReference remoteRepositoryReference, GitServiceReport gitServiceReport)
        {
            return new FetchOptions
            {
                CredentialsProvider = CreateCredentialsProvider(remoteRepositoryReference),
                OnTransferProgress = gitServiceReport.TransferProgressHandler(),
                OnUpdateTips = gitServiceReport.UpdateTipsHandler()
            };
        }
        private CredentialsHandler CreateCredentialsProvider(RemoteRepositoryReference remoteRepositoryReference)
        {
            return (url, user, cred) => new UsernamePasswordCredentials
            {
                Username = tokeProtector.Unprotect(remoteRepositoryReference.EncryptedUserName),
                Password = tokeProtector.Unprotect(remoteRepositoryReference.EncryptedPassword)
            };
        }
    }
}