using System;
using System.Collections.Generic;
using BackupRepositories.Settings.ConfigurationModels;
using BackupRepositories.Settings.ConfigurationModelValidators;
using BackupRepositories.Settings.ServiceSettings;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;

namespace BackupRepositories.Settings
{
    public class SettingsProvider
    {
        private readonly ApplicationConfigurationModel applicationConfigurationModel;

        public SettingsProvider(IConfigurationRoot config)
        {
            applicationConfigurationModel = config.GetSection("ApplicationConfigurationModel").Get<ApplicationConfigurationModel>();
            // Run Model Validation
            var applicationConfigurationModelValidator = new ApplicationConfigurationModelValidator();
            var result = applicationConfigurationModelValidator.Validate(applicationConfigurationModel);
            HasValidationFailures = !result.IsValid;
            ValidationFailures = result.Errors;
        }

        public Dictionary<Type, Object> GetAllSettings()
        {
            Dictionary<Type, Object> dictionary = new Dictionary<Type, object>
            {
                {typeof(DevOpsApiConnectionServiceSettings), GetDevOpsApiConnectionServiceSettings()},
                {typeof(GitServiceSettings), GetGitServiceSettings()},
                {typeof(ProjectServiceSettings), GetProjectServiceSettings()},
                {typeof(LocalRepositoryResourceServiceSettings), GetLocalRepositoryResourceServiceSettings()},
                {typeof(RemoteRepositoryResourceServiceSettings), GetRemoteRepositoryResourceServiceSettings()},
                {typeof(PrtgReportServiceSettings), GetPrtgReportServiceSettings()},
                {typeof(BackupRepositoriesPrtgReportServiceSettings), GetBackupRepositoriesPrtgReportServiceSettings()}
            };
            return dictionary;
        }

        public bool HasValidationFailures { get; private set; }

        public IList<ValidationFailure> ValidationFailures { get; private set; }

        // Build Settings
        private DevOpsApiConnectionServiceSettings GetDevOpsApiConnectionServiceSettings()
        {
            var devOpsApiConnectionServiceSettings = new DevOpsApiConnectionServiceSettings(
                applicationConfigurationModel.DevOpsAuthConfigurationModel.EncryptedAzureDevOpsToken,
                new Uri(applicationConfigurationModel.DevOpsApiConfigurationModel.OrganizationDevOpsUrl));
            return devOpsApiConnectionServiceSettings;
        }

        private GitServiceSettings GetGitServiceSettings()
        {
            var gitServiceSettings =  new GitServiceSettings(
                applicationConfigurationModel.GitConfigurationModel.RemoteOriginName,
                applicationConfigurationModel.GitConfigurationModel.FetchRefSpecs);
            return gitServiceSettings;
        }

        private ProjectServiceSettings GetProjectServiceSettings()
        {
            var projectServiceSettings = new ProjectServiceSettings(
                applicationConfigurationModel.ProjectConfigurationModel.MaxProjectsOffset);
            return projectServiceSettings;
        }

        private LocalRepositoryResourceServiceSettings GetLocalRepositoryResourceServiceSettings()
        {
            var localRepositoryResourceServiceSettings = new LocalRepositoryResourceServiceSettings(
                applicationConfigurationModel.GitConfigurationModel.LocalBasePath);
            return localRepositoryResourceServiceSettings;
        }

        private RemoteRepositoryResourceServiceSettings GetRemoteRepositoryResourceServiceSettings()
        {
            var remoteRepositoryResourceServiceSettings = new RemoteRepositoryResourceServiceSettings(
                applicationConfigurationModel.DevOpsAuthConfigurationModel.EncryptedAzureDevOpsToken);
            return remoteRepositoryResourceServiceSettings;
        }

        private PrtgReportServiceSettings GetPrtgReportServiceSettings()
        {
            var prtgReportServiceSettings = new PrtgReportServiceSettings(
                new Uri(applicationConfigurationModel.PrtgConfigurationModel.PrtgPushServerUri));
            return prtgReportServiceSettings;
        }

        private BackupRepositoriesPrtgReportServiceSettings GetBackupRepositoriesPrtgReportServiceSettings()
        {
            var backupRepositoriesPrtgReportServiceSettings = new BackupRepositoriesPrtgReportServiceSettings(
                applicationConfigurationModel.PrtgConfigurationModel.BackupRepositoriesPrtgSensorId);
            return backupRepositoriesPrtgReportServiceSettings;
        }
    }
}