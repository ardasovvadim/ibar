using Autofac;
using IBAR.Syncer.Infrastructure.Application.Jobs;
using IBAR.Syncer.Infrastructure.Application.Jobs.Data;
using IBAR.Syncer.Infrastructure.Application.Jobs.Fs;
using System.Threading;
using System.Threading.Tasks;

namespace IBAR.SyncerConsole.Initialization
{
    public static class SyncerApplication
    {
        private static IContainer _container;
        private static Thread _worker;

        public static void Init()
        {
            _container = AutofacConfig.GetResolver();
            _worker = new Thread(Work)
            {
                IsBackground = true
            };
        }

        public static void Run()
        {
            _worker.Start();
        }

        public static void Shutdown()
        {
            _worker.Abort();
        }

        private async static void Work()
        {
            using (var lts = _container.BeginLifetimeScope())
            {
                var ftpJob = lts.Resolve<FtpJob>();
                var copyJob = lts.Resolve<CopyFromFtpJob>();
                var fileDeliveryJob = lts.Resolve<FileDeliveryJob>();
                var importJob = lts.Resolve<ImportJob>();
                await Task.WhenAll(
                    Task.Run(() => ftpJob.Run()),
                    // Task.Delay(3 * 60 * 1000).ContinueWith(t => copyJob.Run()),
                    Task.Delay(1000).ContinueWith(t => copyJob.Run()),
                    // Task.Delay(5 * 60 * 1000).ContinueWith(t => fileDeliveryJob.Run()),
                    Task.Delay(1000).ContinueWith(t => fileDeliveryJob.Run()),
                    // Task.Delay(5 * 60 * 1000).ContinueWith(t => importJob.Run())
                    Task.Delay(1000).ContinueWith(t => importJob.Run())
                );
            }
        }
    }
}