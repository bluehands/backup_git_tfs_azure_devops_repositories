using System;

namespace BackupRepositories.Settings.ConfigurationModels
{
    public class Serilog
    {
        public string LogOutputPath { get; set; }
        public string FileLogOutputTemplate { get; set; }
        public int RetainedFileCountLimit { get; set; }
        public string ConsoleOutputTemplate { get; set; }
        public string LogLevel { get; set; }
    }
}