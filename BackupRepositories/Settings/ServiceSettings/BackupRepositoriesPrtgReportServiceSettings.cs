namespace BackupRepositories.Settings.ServiceSettings
{
    public class BackupRepositoriesPrtgReportServiceSettings
    {
        public string BackupRepositoriesPrtgSensorId { get; }

        public BackupRepositoriesPrtgReportServiceSettings(string backupRepositoriesPrtgSensorId)
        {
            BackupRepositoriesPrtgSensorId = backupRepositoriesPrtgSensorId;
        }
    }
}