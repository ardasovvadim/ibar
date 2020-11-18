﻿using Autofac;
using IBAR.St.Toolkit.MemoryCache;
using IBAR.Syncer.Infrastructure.Application.FileSystemProviders;
using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Application.Jobs;
using IBAR.Syncer.Infrastructure.Application.Jobs.Data;
using IBAR.Syncer.Infrastructure.Application.Jobs.Fs;
using IBAR.Syncer.Infrastructure.Application.Model;
using IBAR.Syncer.Infrastructure.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Business.Common.Log;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Business.Services.FileServices.Mock;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Data;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.SyncerConsole.Initialization
{
    public static class AutofacConfig
    {
        public static IContainer GetResolver()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterTypes(containerBuilder);
            return containerBuilder.Build();
        }

        public static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<FtpJob>().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<CopyFromFtpJob>().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<FileDeliveryJob>().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<ImportJob>().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();

            builder.RegisterType<FtpLoader>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<FileNameMatcher>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType(typeof(InMemoryCache<TradeAccountModel>)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(InMemoryCache<TradeFeeTypeModel>)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(InMemoryCache<TradeInstrumentModel>)).AsSelf().SingleInstance();

            builder.RegisterType<TradeModelContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ImportRepository>().As<IImportRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FileNameRegexRepository>().As<IFileNameRegexRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FtpJobRepository>().As<IFtpJobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CopyJobRepository>().As<ICopyJobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ImportJobRepository>().As<IImportJobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DeliveryJobRepository>().As<IDeliveryJobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SyncerInfoRepository>().As<ISyncerInfoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SyncerInfoService>().As<ISyncerInfoService>().PropertiesAutowired().InstancePerLifetimeScope();

#if (DEBUG || STAGE)
            builder.RegisterType<FileSystemProvider>().As<IFileManagerService>();
            builder.RegisterType<CryptoStreamerForFileSystem>().As<ICryptoStreamer>().InstancePerLifetimeScope();
            builder.RegisterType<ExtractFileFromFileSystemService>().As<IExtractFileService>().InstancePerLifetimeScope();
            builder.RegisterType<ZohoApiMockService>().As<IRestApiService>().InstancePerLifetimeScope();
            builder.RegisterType<IFileLogger>().As<IGlobalLogger>().SingleInstance();
#elif (RELEASE || PRODUCTION)
            builder.RegisterType<AzureBlobStorageProvider>().As<IFileManagerService>();
            builder.RegisterType<CryptoStreamerForBlobStorage>().As<ICryptoStreamer>().InstancePerLifetimeScope();
            builder.RegisterType<ExtractFileFromBlobStorageService>().As<IExtractFileService>().InstancePerLifetimeScope();
            builder.RegisterType<ZohoApiService>().As<IRestApiService>().InstancePerLifetimeScope();
            builder.RegisterType<IDbLogger>().As<IGlobalLogger>().SingleInstance();
#endif
        }
    }
}