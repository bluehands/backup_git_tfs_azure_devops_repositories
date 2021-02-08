using System.Threading.Tasks;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BackupRepositories.Services.DevOps
{
    public class DevOpsProjectService
    {
        private readonly DevOpsApiConnectionService connectionService;
        private readonly ProjectServiceSettings projectServiceSettings;

        public DevOpsProjectService(DevOpsApiConnectionService connectionService, ProjectServiceSettings projectServiceSettings)
        {
            this.connectionService = connectionService;
            this.projectServiceSettings = projectServiceSettings;
        }

        public async Task<Result<IPagedList<TeamProjectReference>>> GetAllProjects()
        {
            return await connectionService.GetProjectHttpClient().Bind(async bind =>
            {
                var projects = await bind.GetProjects(null, projectServiceSettings.MaxProjectsOffset).ConfigureAwait(false);
                return Result.Ok(projects);
            }).ConfigureAwait(false);
        }
    }
}