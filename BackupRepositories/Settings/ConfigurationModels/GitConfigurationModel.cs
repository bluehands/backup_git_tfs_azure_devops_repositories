namespace BackupRepositories.Settings.ConfigurationModels
{
    public class GitConfigurationModel
    {
        public string LocalBasePath { get; set; }
        public string RemoteOriginName { get; set; }
        public string FetchRefSpecs { get; set; }
    }
}