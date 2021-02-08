using System;
using System.Threading.Tasks;
using BackupRepositories.Settings.ServiceSettings;
using FunicularSwitch;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace BackupRepositories.Services.DevOps
{
    public class DevOpsApiConnectionService
    {
        private readonly IDataProtector tokenProtector;
        private readonly DevOpsApiConnectionServiceSettings serviceSettings;
        private VssConnection vssConnection;

        public DevOpsApiConnectionService(IDataProtector tokenProtector, DevOpsApiConnectionServiceSettings serviceSettings)
        {
            this.tokenProtector = tokenProtector;
            this.serviceSettings = serviceSettings;
        }

        public async Task<Result<ProjectHttpClient>> GetProjectHttpClient()
        {
            return await AssertConnected().Map(async connection => await connection.GetClientAsync<ProjectHttpClient>().ConfigureAwait(false));
        }

        public async Task<Result<GitHttpClient>> GetGitHttpClient()
        {
            return await AssertConnected().Map(async connection => await connection.GetClientAsync<GitHttpClient>().ConfigureAwait(false));
        }

        public async Task<Result<VssConnection>> AssertConnected()
        {
            try
            {
                if (vssConnection == null)
                {
                    var token = tokenProtector.Unprotect(serviceSettings.EncryptedAzureDevOpsToken);
                    vssConnection = new VssConnection(
                        serviceSettings.OrganizationDevOpsUrl,
                        new VssBasicCredential(
                            token,
                            token));
                }
                if (vssConnection.HasAuthenticated)
                    return Result.Ok(vssConnection);

                await vssConnection.ConnectAsync().ConfigureAwait(false);
                return vssConnection.HasAuthenticated
                    ? Result.Ok(vssConnection)
                    : Result.Error<VssConnection>("Fatal Error: Not authorized to access DevOpsApi endpoint!");
            }
            catch (Exception e)
            {
                return Result.Error<VssConnection>($"{e.Message}");
            }
        }
    }
}