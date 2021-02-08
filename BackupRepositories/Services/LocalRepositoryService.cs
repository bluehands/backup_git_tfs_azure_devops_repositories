using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackupRepositories.Services.RepositoryReferences;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Void = BackupRepositories.Util.Void;

namespace BackupRepositories.Services
{
    public class LocalRepositoryService
    {
        private readonly ILogger<LocalRepositoryService> logger;
        private readonly LocalRepositoryResourceServiceSettings serviceSettings;

        public LocalRepositoryService(ILogger<LocalRepositoryService> logger, LocalRepositoryResourceServiceSettings serviceSettings)
        {
            this.logger = logger;
            this.serviceSettings = serviceSettings;
        }

        public List<LocalRepositoryReference> GetLocalRepositoryReferencesToDelete(List<RemoteRepositoryReference> remoteRepositoryReferences)
        {
            var allLocalReferences = GetAllLocalRepositoryReferences();
            return allLocalReferences.Where(item => 
                !remoteRepositoryReferences.Exists(match => 
                    item.RepositoryName == match.RepositoryName &&
                    item.ProjectName == match.ProjectName)).ToList();
        }

        public void DeleteLocalRepositories(List<LocalRepositoryReference> localRepositoryReferences)
        {
            localRepositoryReferences.ForEach(localRepositoryReference =>
            {
                RemoveRepositoryDirectory(localRepositoryReference);
                RemoveEmptyProjectDirectory(localRepositoryReference);
            });
        }

        public List<LocalRepositoryReference> GetAllLocalRepositoryReferences()
        {
            var localRepositoryReferences = new List<LocalRepositoryReference>();
            var projectDirectories = Directory.GetDirectories(serviceSettings.LocalBasePath);
            projectDirectories.ForEach(projectDirectory=>
            {
                var repositoryDirectories =
                    Directory.GetDirectories(projectDirectory);
                repositoryDirectories.ForEach(repositoryDirectory =>
                {
                    localRepositoryReferences.Add(
                        new LocalRepositoryReference(
                            GetProjectDirectoryNameFromPath(projectDirectory), 
                            GetRepositoryDirectoryNameFromPath(repositoryDirectory)));
                });
            });
            return localRepositoryReferences;
        }

        public Result<Repository> GetGitLibRepository(IRepositoryReference repositoryReference)
        {
            try
            {
               return AssertRepositoryDirectoryExists(repositoryReference).Map(_ => new Repository(GetPathToLocalRepository(repositoryReference)));
            }
            catch (Exception e)
            {
                return Result<Repository>.Error(e.Message);
            }
        }

        public Result<Repository> CreateBareGitLibRepository(IRepositoryReference repositoryReference)
        {
            try
            {
               return AssertRepositoryDirectoryNotExists(repositoryReference).Map(_ => new Repository(Repository.Init(GetPathToLocalRepository(repositoryReference), true)));
            }
            catch (Exception e)
            {
                return Result<Repository>.Error(e.Message);
            }
        }

        public string GetPathToLocalRepository(IRepositoryReference repositoryReference)
        {
            return $"{serviceSettings.LocalBasePath}{Path.DirectorySeparatorChar}{repositoryReference.ProjectName}{Path.DirectorySeparatorChar}{repositoryReference.RepositoryName}.git";
        }

        private Result<Void> AssertRepositoryDirectoryNotExists(IRepositoryReference repositoryReference)
        {
            return !Directory.Exists(GetPathToLocalRepository(repositoryReference))
                ? Result.Ok(new Void())
                : Result<Void>.Error($"A backup for repository: {repositoryReference.RepositoryName} already exists!");
        }

        public bool RepositoryDirectoryExists(IRepositoryReference repositoryReference)
        {
            return Directory.Exists(GetPathToLocalRepository(repositoryReference));
        }

        private Result<Void> AssertRepositoryDirectoryExists(IRepositoryReference repositoryReference)
        {
            return Directory.Exists(GetPathToLocalRepository(repositoryReference))
                ? Result.Ok(new Void())
                : Result<Void>.Error($"A backup for repository: {repositoryReference.RepositoryName} does not exist!");
        }

        private string GetPathToProjectDirectory(IRepositoryReference repositoryReference)
        {
            return $"{serviceSettings.LocalBasePath}{Path.DirectorySeparatorChar}{repositoryReference.ProjectName}";
        }

        private void RemoveRepositoryDirectory(IRepositoryReference repositoryReference)
        {
            var path = GetPathToProjectDirectory(repositoryReference);
            if (!Directory.Exists(path)) return;
            logger.LogInformation($"Remove repository directory: {path}");
            Directory.Delete(path, true);
        }

        private void RemoveEmptyProjectDirectory(IRepositoryReference repositoryReference)
        {
            var path = GetPathToProjectDirectory(repositoryReference);
            if (!Directory.Exists(path)) return;
            if (Directory.GetDirectories(path).Length == 0)
            {
                logger.LogInformation($"Remove project directory: {path}");
                Directory.Delete(path, true);
            }
        }

        private string GetProjectDirectoryNameFromPath(string path)
        {
            return Path.GetFileName(path);
        }

        private string GetRepositoryDirectoryNameFromPath(string path)
        {
            return Path.GetFileName(path).Replace(".git", string.Empty);
        }
    }
}