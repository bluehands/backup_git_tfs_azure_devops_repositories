using System;
using System.Threading.Tasks;
using BackupRepositories.Handlers;
using BackupRepositories.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BackupRepositories
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new DependencyInjectionService().ServiceProvider;
            IExecutionHandler handler;
            switch (args.Length)
            { 
                case 0: 
                    Console.WriteLine("Missing command arguments");
                    Console.WriteLine("Available Commands:");
                    Console.WriteLine("Run-BackupRepositories");
                    Console.WriteLine("Import-AzurePat <PlainToken>");
                    Console.WriteLine("Press enter without input to exit");
                    Console.WriteLine("Reading from console now [timeout 60 seconds]:");
                    var input = ConsoleReader.ReadLine(120000);
                    if (input.Length == 0)
                    {
                        return;
                    }
                    await Main(input.Split(" "));
                    return;
                case 1 when args[0] == "Run-BackupRepositories": 
                    handler = serviceProvider.GetRequiredService<BackupRepositoriesHandler>(); 
                    break;
                case 2 when args[0] == "Import-AzurePat": 
                    handler = serviceProvider.GetRequiredService<ImportAzureDevOpsPatHandler>(); 
                    break;
                case 3 when args[0] == "/?":
                case 4 when args[0] == "?":
                case 5 when args[0] == "--help":
                default:
                    handler = serviceProvider.GetRequiredService<DefaultHandler>();
                    break;
            } 
            await handler.Execute(args);
        }
    }
}
