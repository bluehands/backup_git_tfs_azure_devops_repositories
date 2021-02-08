using System.Threading.Tasks;

namespace BackupRepositories.Handlers
{
    public interface IExecutionHandler
    {
        Task Execute(string[] args);
    }
}