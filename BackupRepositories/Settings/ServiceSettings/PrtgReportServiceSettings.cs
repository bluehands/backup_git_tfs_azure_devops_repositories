using System;

namespace BackupRepositories.Settings.ServiceSettings
{
    public class PrtgReportServiceSettings
    {
        public Uri PrtgPushServerUri { get; }

        public PrtgReportServiceSettings(Uri prtgPushServerUri)
        {
            PrtgPushServerUri = prtgPushServerUri;
        }
    }
}