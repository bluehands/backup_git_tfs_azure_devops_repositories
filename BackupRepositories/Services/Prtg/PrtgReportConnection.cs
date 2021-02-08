using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BackupRepositories.Services.Prtg
{
    public class PrtgReportConnection : IPrtgReportConnection
    {
        private readonly ILogger<PrtgReportConnection> logger;
        private readonly PrtgReportServiceSettings serviceSettings;
        private BackupRepositories.Services.Prtg.Prtg prtg;

        public PrtgReportConnection(ILogger<PrtgReportConnection> logger, PrtgReportServiceSettings serviceSettings)
        {
            this.logger = logger;
            this.serviceSettings = serviceSettings;
        }

        public void AddResult(PrtgResult prtgResult)
        {
            if (prtg == null)
            {
                prtg = new BackupRepositories.Services.Prtg.Prtg { Text = "", Error = "0" };
            }
            prtg.Results.Add(prtgResult);
        }

        public void SetFailure(string message)
        {
            prtg = new BackupRepositories.Services.Prtg.Prtg { Text = message, Error = "1" };
        }

        public string Publish()
        {
            if (prtg == null)
            {
                prtg = new BackupRepositories.Services.Prtg.Prtg {Text = "Published without content", Error = "1"};
            }
            return JsonConvert.SerializeObject(new PrtgRoot(prtg));
        }

        public async Task<Result<string>> PublishToServer(string sensorId)
        {
            try
            {
                var prtgString = Publish();
                var client = new HttpClient();
                var response = await client.PostAsync(new Uri(serviceSettings.PrtgPushServerUri, sensorId),
                    new StringContent(prtgString, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Successfully pushed report to prtg server! Response: {content}");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    logger.LogWarning($"Failed to push report to prtg server! Response: {content}");
                }

                client.Dispose();
                return prtgString;
            }
            catch (Exception e)
            {
                return Result.Error<string>(e.Message);
            }
        }
    }
}