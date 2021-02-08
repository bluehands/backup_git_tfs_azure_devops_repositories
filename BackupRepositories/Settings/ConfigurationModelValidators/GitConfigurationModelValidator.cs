using System.IO;
using BackupRepositories.Settings.ConfigurationModels;
using FluentValidation;

namespace BackupRepositories.Settings.ConfigurationModelValidators
{
    public class GitConfigurationModelValidator : AbstractValidator<GitConfigurationModel>
    {
        public GitConfigurationModelValidator()
        {
            RuleFor(model => model.LocalBasePath).Must(path => Directory.Exists(path))
                .WithMessage("Path to root backup directory does not exist");
            RuleFor(model => model.RemoteOriginName).NotEmpty();
            RuleFor(model => model.FetchRefSpecs).NotEmpty();
        }
    }
}