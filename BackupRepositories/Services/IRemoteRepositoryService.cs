using System.Collections.Generic;
using System.Threading.Tasks;
using BackupRepositories.Services.RepositoryReferences;
using FunicularSwitch;

namespace BackupRepositories.Services
{
    public interface IRemoteRepositoryService
    {
        Task<Result<List<RemoteRepositoryReference>>> GetAllRemoteRepositoryReferences();
    }
}