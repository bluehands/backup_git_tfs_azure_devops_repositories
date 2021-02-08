using System.IO;
using System.Threading.Tasks;
using BackupRepositories.Settings.ConfigurationModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

namespace BackupRepositories.Handlers
{
    public class ImportAzureDevOpsPatHandler : IExecutionHandler
    {
        private readonly ILogger<ImportAzureDevOpsPatHandler> logger;
        private readonly IDataProtector tokenProtector;
        public AppSettingsRoot AppSettingsRoot { get; private set; }

        public ImportAzureDevOpsPatHandler(ILogger<ImportAzureDevOpsPatHandler> logger, IDataProtector tokenProtector)
        {
            this.logger = logger;
            this.tokenProtector = tokenProtector;
        }

        public Task Execute(string[] args)
        {
            var deserializer = new Deserializer();
            var serializer = new Serializer();

            using (var streamReader = new StreamReader("appsettings.yml"))
            {
                AppSettingsRoot = deserializer.Deserialize<AppSettingsRoot>(streamReader);
            }
            AppSettingsRoot.ApplicationConfigurationModel.DevOpsAuthConfigurationModel.EncryptedAzureDevOpsToken =
                tokenProtector.Protect(args[1]);
            using (var streamWriter = new StreamWriter("appsettings.yml"))
            {
                serializer.Serialize(streamWriter, AppSettingsRoot);
            }
            logger.LogInformation("Wrote EncryptedAzureDevOpsToken to appsettings.yml");

            return Task.CompletedTask;
        }
    }
}