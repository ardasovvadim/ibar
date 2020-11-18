using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IBAR.Syncer.Application.Jobs.Data;
using IBAR.Syncer.Application.Jobs.Fs;
using IBAR.Syncer.Wcf;

namespace IBAR.Syncer.Initialization
{
    public static class SyncerApplication
    {
        private static IContainer _container;
        private static Thread _worker;
        //private static readonly Service _service = new Service();


        public static void Init()
        {
            InitIoc();
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
            //using (var lts = _container.BeginLifetimeScope())
            //{
            //    // TODO : uncomment
            //    var ftpJob = lts.Resolve<FtpJob>();
            //    var copyJob = lts.Resolve<FsCopyFromFtpJob>();
            //    var importJob = lts.Resolve<ImportJob>();
            //    // _service.GetInstance(null, cop, null);
            //    await Task.WhenAll(
            //        Task.Run(() => ftpJob.Run()),
            //        Task.Run(() => copyJob.Run()),
            //        Task.Run(() => importJob.Run())
            //    );
            //}
        }

        private static void InitIoc()
        {
            var builder = new ContainerBuilder();

            InitTradeDataLayer(builder);
            _container = builder.Build();
        }

        private static void InitTradeDataLayer(ContainerBuilder builder)
        {
            //TradeModel.Data.IoC.Register(builder);
            //TradeModel.Business.IoC.Register(builder);
            IoC.Register(builder);
        }
    }
}