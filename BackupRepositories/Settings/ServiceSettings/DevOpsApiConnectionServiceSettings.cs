using System;

namespace BackupRepositories.Settings.ServiceSettings
{
    public class DevOpsApiConnectionServiceSettings
    {
        public DevOpsApiConnectionServiceSettings(string encryptedAzureDevOpsToken, Uri organizationDevOpsUrl)
        {
            EncryptedAzureDevOpsToken = encryptedAzureDevOpsToken;
            OrganizationDevOpsUrl = organizationDevOpsUrl;
        }
        public string EncryptedAzureDevOpsToken { get; }
        public Uri OrganizationDevOpsUrl { get; }
    }
}