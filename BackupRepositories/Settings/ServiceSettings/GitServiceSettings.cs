namespace BackupRepositories.Settings.ServiceSettings
{
    public class GitServiceSettings
    {
        public GitServiceSettings(string remoteOriginName, string fetchRefSpecs)
        {
            RemoteOriginName = remoteOriginName;
            FetchRefSpecs = fetchRefSpecs;
        }

        public string RemoteOriginName { get;}
        public string FetchRefSpecs { get; }
    }
}