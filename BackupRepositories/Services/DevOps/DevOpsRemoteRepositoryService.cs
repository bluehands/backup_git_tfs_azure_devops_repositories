using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupRepositories.Services.RepositoryReferences;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BackupRepositories.Services.DevOps
{
    public class DevOpsRemoteRepositoryService : IRemoteRepositoryService
    {
        private readonly ILogger<DevOpsRemoteRepositoryService> logger;
        private readonly DevOpsApiConnectionService devOpsApiConnectionService;
        private readonly DevOpsProjectService devOpsDevOpsProjectService;
        private readonly RemoteRepositoryResourceServiceSettings serviceSettings;

        public DevOpsRemoteRepositoryService(
            ILogger<DevOpsRemoteRepositoryService> logger,
            DevOpsApiConnectionService devOpsApiConnectionService,
            DevOpsProjectService devOpsDevOpsProjectService,
            RemoteRepositoryResourceServiceSettings serviceSettings)
        {
            this.logger = logger;
            this.devOpsApiConnectionService = devOpsApiConnectionService;
            this.devOpsDevOpsProjectService = devOpsDevOpsProjectService;
            this.serviceSettings = serviceSettings;
        }

        public Task<Result<List<RemoteRepositoryReference>>> GetAllRemoteRepositoryReferences() =>
            GetAllRepositoriesFromAllDevOpsProjects()
                .Map(repositories =>
                    CreateRemoteRepositoryReferences(repositories));

        private List<RemoteRepositoryReference> CreateRemoteRepositoryReferences(List<GitRepository> repositories)
        {
            var remoteRepositoryReferences = new List<RemoteRepositoryReference>();
            remoteRepositoryReferences.AddRange(repositories.Select(gitRepository =>
                new RemoteRepositoryReference(
                    gitRepository.ProjectReference.Name,
                    gitRepository.Name,
                    gitRepository.RemoteUrl,
                    serviceSettings.EncryptedAzureDevOpsToken,
                    serviceSettings.EncryptedAzureDevOpsToken,
                    RemoteSource.AzureDevOps)));
            return remoteRepositoryReferences;
        }

        private Task<Result<List<GitRepository>>> GetAllRepositoriesFromAllDevOpsProjects() =>
            devOpsDevOpsProjectService
                .GetAllProjects()
                .Bind(GetAllGitRepositories);

        private Task<Result<List<GitRepository>>> GetAllGitRepositories(IPagedList<TeamProjectReference> projects)
        {
            return devOpsApiConnectionService.GetGitHttpClient()
                .Map(gitHttpClient =>
                    QueryGitRepositoriesFromDevOps(projects, gitHttpClient));
        }

        private static async Task<List<GitRepository>> QueryGitRepositoriesFromDevOps(IPagedList<TeamProjectReference> projects,
            GitHttpClient gitHttpClient)
        {
            return (await Task.WhenAll(projects.Select(project =>
                    gitHttpClient.GetRepositoriesAsync(project.Id))).ConfigureAwait(false))
                .SelectMany(element => element)
                .ToList();
        }
    }
}