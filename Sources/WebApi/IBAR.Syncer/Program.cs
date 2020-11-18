using System;
using System.ServiceModel;
using IBAR.Syncer.Initialization;
using IBAR.Syncer.Wcf;
using Microsoft.Azure.WebJobs;
using NLog;

namespace IBAR.Syncer
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static Logger logg = LogManager.GetLogger("Logg");

        static void Main()
        {
            // ServiceHost host = new ServiceHost(typeof(Service));
            // host.Open();
            
            Console.WriteLine("Server is running...");
            
            Run();
            
            Console.WriteLine("Press enter to stop server");
            
            while (Console.ReadKey().Key != ConsoleKey.Enter);
            // host.Close();

            // RunAsConsoleHost();
        }

        private static void RunAsConsoleHost()
        {
            Run();
            Console.ReadKey();

            SyncerApplication.Shutdown();
        }

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static void RunAsJob()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        private static void Run()
        {
            SyncerApplication.Init();
            SyncerApplication.Run();
        }
    }
}