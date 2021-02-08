namespace BackupRepositories.Settings.ServiceSettings
{
    public class LocalRepositoryResourceServiceSettings
    {
        public string LocalBasePath { get; }
        public LocalRepositoryResourceServiceSettings(string localBasePath)
        {
            LocalBasePath = localBasePath;
        }
    }
}