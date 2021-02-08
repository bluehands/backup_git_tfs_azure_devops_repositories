namespace BackupRepositories.Settings.ConfigurationModels
{
    public class AppSettingsRoot
    {
        public Serilog Serilog { get; set; }
        public DataProtection DataProtection { get; set; }
        public ApplicationConfigurationModel ApplicationConfigurationModel { get; set; }
    }
}