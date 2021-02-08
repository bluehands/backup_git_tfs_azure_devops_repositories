namespace BackupRepositories.Services.RepositoryReferences
{
    public class RemoteRepositoryReference : IRepositoryReference
    {
        public string ProjectName { get; }

        public string RepositoryName { get; }

        public string EncryptedUserName { get; }

        public string EncryptedPassword { get; }

        public string RemoteUrl { get; }

        public RemoteSource RemoteSource { get; }

        public RemoteRepositoryReference(string projectName, string repositoryName, string remoteUrl, string encryptedUserName, string encryptedPassword, RemoteSource remoteSource)
        {
            ProjectName = projectName;
            RepositoryName = repositoryName;
            RemoteUrl = remoteUrl;
            EncryptedUserName = encryptedUserName;
            EncryptedPassword = encryptedPassword;
            RemoteSource = remoteSource;
        }
    }
}