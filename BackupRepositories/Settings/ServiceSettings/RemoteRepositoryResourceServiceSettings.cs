namespace BackupRepositories.Settings.ServiceSettings
{
    public class RemoteRepositoryResourceServiceSettings
    {
        public string EncryptedAzureDevOpsToken { get; }
        public RemoteRepositoryResourceServiceSettings(string encryptedAzureDevOpsToken)
        {
            EncryptedAzureDevOpsToken = encryptedAzureDevOpsToken;
        }
    }
}