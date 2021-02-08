using System;
using System.Collections.Generic;
using System.Text;
using BackupRepositories.Services.RepositoryReferences;
using LibGit2Sharp.Handlers;

namespace BackupRepositories.Services
{
    public class GitServiceReport
    {
        public ReportType ReportType { get; }
        public string BackupRepositoryPath { get; }
        public RemoteRepositoryReference RemoteRepositoryReference { get; }

        public GitServiceReport(ReportType reportType, string backupRepositoryPath, RemoteRepositoryReference remoteRepositoryReference)
        {
            ReportType = reportType;
            BackupRepositoryPath = backupRepositoryPath;
            RemoteRepositoryReference = remoteRepositoryReference;
        }

        public int TotalObjects { get; set; } = 0;
        public int ReceivedObjects { get; set; } = 0;
        public int IndexedObjects { get; set; } = 0;
        public long ReceivedBytes { get; set; } = 0;

        public Dictionary<string, string> UpdatedBranches { get; set; } = new Dictionary<string, string>();

        public string CreateLog()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("GitReport:");
            stringBuilder.AppendLine($"{nameof(ReportType)}:              {ReportType}");
            stringBuilder.AppendLine($"{nameof(RemoteRepositoryReference.RepositoryName)}:          {RemoteRepositoryReference.RepositoryName}");
            stringBuilder.AppendLine($"{nameof(RemoteRepositoryReference.RemoteUrl)}:               {RemoteRepositoryReference.RemoteUrl}");
            stringBuilder.AppendLine($"{nameof(BackupRepositoryPath)}:    {BackupRepositoryPath}");
            stringBuilder.AppendLine($"{nameof(IndexedObjects)}:          {ReceivedObjects} of {TotalObjects}");
            stringBuilder.AppendLine($"{nameof(ReceivedBytes)}:           {ReceivedBytes}");
            if (UpdatedBranches.Count > 0)
            {
                stringBuilder.AppendLine("UpdatedBranches:");
                stringBuilder.Append(string.Join(Environment.NewLine, UpdatedBranches));
            }
            else
            {
                stringBuilder.Append("UpdatedBranches:         None");
            }
            return stringBuilder.ToString();
        }

        public TransferProgressHandler TransferProgressHandler()
        {
            return progress =>
            {
                ReceivedObjects = progress.ReceivedObjects;
                IndexedObjects = progress.IndexedObjects;
                ReceivedBytes = progress.ReceivedBytes;
                TotalObjects = progress.TotalObjects;
                return true;
            };
        }

        public UpdateTipsHandler UpdateTipsHandler()
        {
            return (name, id, newId) =>
            {
                UpdatedBranches.Add(name, newId.ToString());
                return true;
            };
        }
    }
    public enum ReportType
    {
        CloneMirror,
        FetchFromRemote
    }
}