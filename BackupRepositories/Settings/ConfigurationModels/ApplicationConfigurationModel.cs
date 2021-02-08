namespace BackupRepositories.Settings.ConfigurationModels
{
    public class ApplicationConfigurationModel
    {
        public DevOpsAuthConfigurationModel DevOpsAuthConfigurationModel { get; set; }
        public DevOpsApiConfigurationModel DevOpsApiConfigurationModel { get; set; }
        public GitConfigurationModel GitConfigurationModel { get; set; }
        public ProjectConfigurationModel ProjectConfigurationModel { get; set; }
        public PrtgConfigurationModel PrtgConfigurationModel { get; set; }
    }
}