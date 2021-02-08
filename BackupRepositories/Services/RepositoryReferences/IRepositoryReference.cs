namespace BackupRepositories.Services.RepositoryReferences
{
    public interface IRepositoryReference
    {
        string ProjectName { get;}
        string RepositoryName { get; }
    }

    public enum RemoteSource 
    {
        AzureDevOps,
        Github
    }
}