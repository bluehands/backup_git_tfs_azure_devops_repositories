using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BackupRepositories.Handlers
{
    public class DefaultHandler : IExecutionHandler
    {
        private readonly ILogger<DefaultHandler> logger;

        public DefaultHandler(ILogger<DefaultHandler> logger)
        {
            this.logger = logger;
        }
        public Task Execute(string[] args)
        {
            logger.LogWarning($"No Command for Parameters: {string.Join(",", args)}");
            return Task.CompletedTask;
        }
    }
}