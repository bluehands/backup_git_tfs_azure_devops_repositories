using System;
using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class PrtgConfigurationModelValidator : AbstractValidator<PrtgConfigurationModel>
    {
        public PrtgConfigurationModelValidator()
        {
            RuleFor(model => model.PrtgPushServerUri).NotEmpty().Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
            RuleFor(model => model.BackupRepositoriesPrtgSensorId).NotEmpty();
        }
    }
}