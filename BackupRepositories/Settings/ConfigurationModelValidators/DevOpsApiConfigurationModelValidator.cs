using System;
using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class DevOpsApiConfigurationModelValidator : AbstractValidator<DevOpsApiConfigurationModel>
    {
        public DevOpsApiConfigurationModelValidator()
        {
            RuleFor(model => model.OrganizationDevOpsUrl)
                .NotEmpty()
                .Must(uriString => Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
                .WithMessage("OrganizationDevOpsUrl must be a absolute uri string");
            RuleFor(model => model.OrganizationDevOpsName).NotEmpty();
        }
    }
}