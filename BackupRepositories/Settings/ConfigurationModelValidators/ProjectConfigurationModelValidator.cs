using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class ProjectConfigurationModelValidator : AbstractValidator<ProjectConfigurationModel>
    {
        public ProjectConfigurationModelValidator()
        {
            RuleFor(model => model.MaxProjectsOffset).NotEmpty();
        }
    }
}