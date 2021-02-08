using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BackupRepositories.Services.RepositoryReferences;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using Microsoft.Extensions.Logging;

namespace BackupRepositories.Services.Prtg
{

    public class PrtgReportService : IReportService
    {
        private readonly ILogger<PrtgReportService> logger;
        private readonly PrtgReportConnection prtgReportConnection;
        private readonly BackupRepositoriesPrtgReportServiceSettings serviceSettings;

        private readonly StringBuilder errorMessagesBuilder;
        private int errors;

        private int updatedBackupRepositories;
        private int cloneUpdatedBackupRepositories;
        private int fetchUpdatedBackupRepositories;
        private int totalRemoteObjects;
        private int totalReceivedObjects;
        private int totalIndexedObjects;
        private long receivedBytes;
        private int updatedBranches;

        private int remoteRepositories;
        private int backupRepositories;
        private int backupRepositoriesBeforeDelete;
        private int deletedBackupRepositories;

        private long elapsedSeconds;
        


        public PrtgReportService(
            ILogger<PrtgReportService> logger, 
            PrtgReportConnection prtgReportConnection,
            BackupRepositoriesPrtgReportServiceSettings serviceSettings)
        {
            this.logger = logger;
            this.prtgReportConnection = prtgReportConnection;
            this.serviceSettings = serviceSettings;

            this.errorMessagesBuilder = new StringBuilder();
            this.errors = 0;

            this.updatedBackupRepositories = 0;
            this.cloneUpdatedBackupRepositories = 0;
            this.fetchUpdatedBackupRepositories = 0;
            this.totalRemoteObjects = 0;
            this.totalReceivedObjects = 0;
            this.totalIndexedObjects = 0;
            this.receivedBytes = 0;
            this.updatedBranches = 0;

            this.remoteRepositories = 0;

            this.backupRepositories = 0;
            this.backupRepositoriesBeforeDelete = 0;
            this.deletedBackupRepositories = 0;

            this.elapsedSeconds = 0;

        }

        public void AddReportForGitServiceReport(GitServiceReport gitServiceReport)
        {
            updatedBackupRepositories++;
            switch (gitServiceReport.ReportType)
            {
                case ReportType.CloneMirror:
                    cloneUpdatedBackupRepositories++;
                    break;
                case ReportType.FetchFromRemote:
                    fetchUpdatedBackupRepositories++;
                    break;
                default:
                    logger.LogWarning("ReportType missing!");
                    break;
            }
            totalRemoteObjects += gitServiceReport.TotalObjects;
            totalReceivedObjects += gitServiceReport.ReceivedObjects;
            totalIndexedObjects += gitServiceReport.IndexedObjects;
            receivedBytes += gitServiceReport.ReceivedBytes;
            updatedBranches += gitServiceReport.UpdatedBranches.Count;
        }

        public void AddReportForRemoteRepositories(List<RemoteRepositoryReference> repositoryReferences)
        {
            remoteRepositories = repositoryReferences.Count;
        }

        public void AddReportForLocalRepositories(List<LocalRepositoryReference> repositoryReferences)
        {
            backupRepositories = repositoryReferences.Count;
        }

        public void AddReportForLocalRepositoriesBeforeDelete(List<LocalRepositoryReference> repositoryReferences)
        {
            backupRepositoriesBeforeDelete = repositoryReferences.Count;
        }

        public void AddReportForDeletedLocalRepositories(List<LocalRepositoryReference> repositoryReferences)
        {
            deletedBackupRepositories = repositoryReferences.Count;
        }

        public void AddReportForError(string message)
        {
            errorMessagesBuilder.Append(string.Join("; ", message));
            errors++;
        }

        public void AddReportForElapsedTimeOfBackupInMilliseconds(long milliseconds)
        {
            elapsedSeconds = milliseconds/1000;
        }

        public Task<Result<string>> Publish()
        {
            if (errors > 0)
            {
                prtgReportConnection.SetFailure($"{nameof(errors)}:{errors} Messages: {errorMessagesBuilder}");
            }
            else
            {
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(elapsedSeconds), elapsedSeconds.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.TimeSeconds)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(updatedBackupRepositories), updatedBackupRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(cloneUpdatedBackupRepositories), cloneUpdatedBackupRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(fetchUpdatedBackupRepositories), fetchUpdatedBackupRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(totalRemoteObjects), totalRemoteObjects.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(totalReceivedObjects), totalReceivedObjects.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(totalIndexedObjects), totalIndexedObjects.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(receivedBytes), receivedBytes.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.BytesBandwidth)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(updatedBranches), updatedBranches.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(remoteRepositories), remoteRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(backupRepositories), backupRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(backupRepositoriesBeforeDelete), backupRepositoriesBeforeDelete.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
                prtgReportConnection.AddResult(
                    new PrtgResult(nameof(deletedBackupRepositories), deletedBackupRepositories.ToString())
                        .SetMode(PrtgResultMode.Absolute)
                        .SetShowChart()
                        .SetNotifyChanged()
                        .SetUnit(PrtgResultUnit.Count)
                    );
            }
            return prtgReportConnection.PublishToServer(serviceSettings.BackupRepositoriesPrtgSensorId);
        }
    }
}