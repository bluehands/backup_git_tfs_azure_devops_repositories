namespace BackupRepositories.Services.RepositoryReferences
{
    public class LocalRepositoryReference : IRepositoryReference
    {
        public string ProjectName { get; private set; }
        public string RepositoryName { get; private set; }

        public LocalRepositoryReference(string projectName, string repositoryName)
        {
            ProjectName = projectName;
            RepositoryName = repositoryName;
        }
    }
}