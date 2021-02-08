using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class ApplicationConfigurationModelValidator : AbstractValidator<ApplicationConfigurationModel>
    {
        public ApplicationConfigurationModelValidator()
        {
            RuleFor(model => model.GitConfigurationModel).NotEmpty().SetValidator(new GitConfigurationModelValidator());
            RuleFor(model => model.DevOpsApiConfigurationModel).NotEmpty().SetValidator(new DevOpsApiConfigurationModelValidator());
            RuleFor(model => model.DevOpsAuthConfigurationModel).NotEmpty().SetValidator(new DevOpsAuthModelValidator());
            RuleFor(model => model.ProjectConfigurationModel).NotEmpty().SetValidator(new ProjectConfigurationModelValidator());
            RuleFor(model => model.PrtgConfigurationModel).NotEmpty().SetValidator(new PrtgConfigurationModelValidator());
        }
    }
}