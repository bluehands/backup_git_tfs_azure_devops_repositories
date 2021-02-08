namespace BackupRepositories.Settings.ServiceSettings
{
    public class ProjectServiceSettings
    {
        public int MaxProjectsOffset { get; }

        public ProjectServiceSettings(int maxProjectsOffset)
        {
            MaxProjectsOffset = maxProjectsOffset;
        }
    }
}