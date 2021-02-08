using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class DevOpsAuthModelValidator : AbstractValidator<DevOpsAuthConfigurationModel>
    {
        public DevOpsAuthModelValidator()
        {
            RuleFor(model => model.EncryptedAzureDevOpsToken).NotEmpty().WithMessage("EncryptedAzureDevOpsToken has wrong Format or is missing");
        }
    }
}