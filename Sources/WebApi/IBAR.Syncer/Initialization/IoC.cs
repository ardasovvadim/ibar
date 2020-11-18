using Autofac;
using IBAR.St.Toolkit.MemoryCache;
using IBAR.Syncer.Application.FileSystemProviders;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Application.Jobs.Data;
using IBAR.Syncer.Application.Jobs.Fs;
using IBAR.Syncer.Application.Model;
using IBAR.Syncer.Tools.FileSystem.Ftp;
using IBAR.Syncer.Wcf;

namespace IBAR.Syncer.Initialization
{
    public static class IoC
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterType<FtpJob>()
                .As<FtpJob>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FsCopyFromFtpJob>()
                .As<FsCopyFromFtpJob>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ImportJob>()
                .As<ImportJob>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FtpLoader>()
                .As<FtpLoader>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FileNameMatcher>()
             .As<FileNameMatcher>()
             .InstancePerLifetimeScope();

            builder.RegisterType(typeof(InMemoryCache<TradeAccountModel>))
               .As(typeof(InMemoryCache<TradeAccountModel>))
               .SingleInstance();

            builder.RegisterType(typeof(InMemoryCache<TradeAccountStatusModel>))
               .As(typeof(InMemoryCache<TradeAccountStatusModel>))
               .SingleInstance();

            builder.RegisterType(typeof(InMemoryCache<TradeFeeTypeModel>))
               .As(typeof(InMemoryCache<TradeFeeTypeModel>))
               .SingleInstance();

            builder.RegisterType(typeof(InMemoryCache<TradeInstrumentModel>))
               .As(typeof(InMemoryCache<TradeInstrumentModel>))
               .SingleInstance();

            builder.RegisterType(typeof(InMemoryCache<TradeMasterAccountModel>))
               .As(typeof(InMemoryCache<TradeMasterAccountModel>))
               .SingleInstance();

#if (DEBUG || STAGE)
            builder.RegisterType<FileSystemProvider>().As<IFileSystemManager>();
#elif RELEASE
            builder.RegisterType<AzureBlobStorageProvider>().As<IFileSystemManager>();
#endif

            builder.RegisterType<Service>().As<IContract>();
        }
    }
}