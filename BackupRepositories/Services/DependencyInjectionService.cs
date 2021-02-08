using System;
using System.IO;
using BackupRepositories.Handlers;
using BackupRepositories.Services.DataProtection;
using BackupRepositories.Services.DevOps;
using BackupRepositories.Services.Prtg;
using BackupRepositories.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using Serilog;

namespace BackupRepositories.Services
{
    public class DependencyInjectionService
    {
        private readonly IConfigurationRoot appSettingsRoot;
        private readonly ILogger logger;

        public ServiceProvider ServiceProvider { get; set; }

        public DependencyInjectionService()
        {
            appSettingsRoot = LoadAppsettings();
            logger = Log.Logger = ConfigureSerilog().CreateLogger();

            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServiceSettings(serviceCollection);
                ConfigureServices(serviceCollection);
                ConfigureDataProtection(serviceCollection);

                ServiceProvider = serviceCollection.BuildServiceProvider();
            }
            catch (Exception e)
            {
                logger.Error($"Error: {e}");
                throw;
            }
        }
        private IConfigurationRoot LoadAppsettings()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("appsettings.yml") // load appsettings yaml
                .AddUserSecrets<Program>(); // user secret to avoid check in of appsettings section: ApplicationConfigurationModel:DevOpsAuthConfigurationModel:EncryptedAzureDevOpsToken: <encrypted token>
            return builder.Build();
        }

        private void ConfigureServiceSettings(IServiceCollection services)
        {
            var settingsProvider = new SettingsProvider(appSettingsRoot);
            if (settingsProvider.HasValidationFailures)
            {
                throw new Exception(string.Join(",", settingsProvider.ValidationFailures));
            }

            settingsProvider.GetAllSettings().ForEach(element =>
            {
                services.AddSingleton(element.Key, element.Value);
            });
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
                {
                    builder.AddSerilog(dispose:true);
                })

                // Register core services
                .AddTransient<GitService>()
                .AddTransient<LocalRepositoryService>()

                // Register devops specific services
                .AddSingleton<DevOpsApiConnectionService>()
                .AddTransient<DevOpsProjectService>()
                .AddTransient<IRemoteRepositoryService, DevOpsRemoteRepositoryService>()

                // Register report service for monitoring
                .AddTransient<PrtgReportConnection>()
                .AddTransient<IReportService, PrtgReportService>()
                
                // Register handlers
                .AddTransient<BackupRepositoriesHandler>()
                .AddTransient<ImportAzureDevOpsPatHandler>()
                .AddTransient<DefaultHandler>();
        }

        private void ConfigureDataProtection(IServiceCollection services)
        {
            var registryPath = GetAppsettingsValue("DataProtection:RegistryCurrentUserSubKeyPath");
            services
                .AddSingleton<IDataProtector, AzureDevOpsTokenProtector>()
                .AddDataProtection()
                .ProtectKeysWithDpapiNG()
                .PersistKeysToRegistry(Registry.CurrentUser.OpenSubKey(registryPath, true));
        }

        private LoggerConfiguration ConfigureSerilog()
        {
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.WriteTo.RollingFile(
                GetAppsettingsValue("Serilog:LogOutputPath"),
                outputTemplate: GetAppsettingsValue("Serilog:FileLogOutputTemplate"),
                retainedFileCountLimit: int.Parse(GetAppsettingsValue("Serilog:RetainedFileCountLimit"))
            );
            loggerConfiguration.WriteTo.Console(
                outputTemplate: GetAppsettingsValue("Serilog:ConsoleOutputTemplate"));

            switch (GetAppsettingsValue("Serilog:LogLevel"))
            {
                case "Information":
                    loggerConfiguration.MinimumLevel.Information();
                    break;
                case "Debug":
                    loggerConfiguration.MinimumLevel.Debug();
                    break;
                case "Verbose":
                    loggerConfiguration.MinimumLevel.Verbose();
                    break;
                default:
                    loggerConfiguration.MinimumLevel.Information();
                    break;
            }

            return loggerConfiguration;
        }

        private string GetAppsettingsValue(string sectionKey)
        {
           return appSettingsRoot.GetSection(sectionKey).Value;
        }
    }
}