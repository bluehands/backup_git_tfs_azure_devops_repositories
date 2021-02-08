using System.Collections.Generic;
using System.Threading.Tasks;
using BackupRepositories.Services.RepositoryReferences;
using FunicularSwitch;

namespace BackupRepositories.Services
{
    public interface IReportService
    {
        void AddReportForGitServiceReport(GitServiceReport gitServiceReport);
        void AddReportForRemoteRepositories(List<RemoteRepositoryReference> repositoryReferences);
        void AddReportForLocalRepositories(List<LocalRepositoryReference> repositoryReferences);
        void AddReportForLocalRepositoriesBeforeDelete(List<LocalRepositoryReference> repositoryReferences);
        void AddReportForDeletedLocalRepositories(List<LocalRepositoryReference> repositoryReferences);
        void AddReportForError(string message);
        void AddReportForElapsedTimeOfBackupInMilliseconds(long milliseconds);
        Task<Result<string>> Publish();
    }
}