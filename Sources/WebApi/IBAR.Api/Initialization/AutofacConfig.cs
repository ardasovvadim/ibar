using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using IBAR.Api.Filters;
using IBAR.TradeModel.Business.Common.Log;
using IBAR.TradeModel.Business.Providers;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Business.Services.Wcf;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Data;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.Api.Initialization
{
    public class AutofacConfig
    {
        public static IDependencyResolver GetResolver()
        {
            var builder = new ContainerBuilder();
            RegisterServices(builder);
            return new AutofacWebApiDependencyResolver(builder.Build());
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            builder.RegisterWebApiModelBinderProvider();
            builder.Register(c => new IdentityFilter(c.Resolve<IIdentityService>()))
                .AsWebApiActionFilterForAllControllers()
                .InstancePerRequest();
            builder.Register(c => new ActionContextFilter(c.Resolve<IActionContextAccessor>()))
                .AsWebApiActionFilterForAllControllers()
                .InstancePerRequest();
            builder.RegisterType<AsyncActionAttribute>().PropertiesAutowired();

            builder.RegisterType<ExceptionHandlerFilter>().AsWebApiExceptionFilterForAllControllers().PropertiesAutowired().InstancePerRequest();
            builder.RegisterType<LoggingFilterAttribute>().AsWebApiActionFilterForAllControllers().PropertiesAutowired().InstancePerRequest();

            builder.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<MasterAccountService>().As<IMasterAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<FtpCredentialService>().As<IFtpCredentialService>().InstancePerLifetimeScope();
            builder.RegisterType<SourceFileService>().As<ISourcesFilesService>().InstancePerLifetimeScope();
            builder.RegisterType<WcfService>().As<IWcfService>().InstancePerLifetimeScope();
            builder.RegisterType<TradeAccountService>().As<ITradeAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerLifetimeScope();
            builder.RegisterType<TotalAccountService>().As<ITotalAccountService>().InstancePerRequest();
            builder.RegisterType<ExceptionService>().As<IExceptionService>().InstancePerLifetimeScope();
            builder.RegisterType<InviteService>().As<IInviteService>().InstancePerLifetimeScope();
            builder.RegisterType<IncomeService>().As<IIncomeService>().InstancePerLifetimeScope();
            builder.RegisterType<IdentityService>().As<IIdentityService>().InstancePerRequest();
            builder.RegisterType<MessengerService>().As<IMessengerService>().InstancePerLifetimeScope();
            builder.RegisterType<LogInfoService>().As<ILogInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<JwtTokenProvider>().As<IJwtTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerRequest();
            builder.RegisterType<AnalyticsService>().As<IAnalyticsService>().InstancePerLifetimeScope();
            builder.RegisterType<SyncerInfoService>().As<ISyncerInfoService>().InstancePerLifetimeScope();

            builder.RegisterType<TradeModelContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InviteRepository>().As<IInviteRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TradeAccountRepository>().As<ITradeAccountRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MasterAccountRepository>().As<IMasterAccountRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TradeRepository>().As<ITradeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ImportRepository>().As<IImportRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FtpCredentialRepository>().As<IFtpCredentialRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LogInfoRepository>().As<ILogInfoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RoleRepository>().As<IRoleRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TradExeRepository>().As<ITradeExeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<IDbApiLogger>().As<IApiLogger>().SingleInstance();
            builder.RegisterType<IDbLogger>().As<IGlobalLogger>().SingleInstance();
            builder.RegisterType<SyncerInfoRepository>().As<ISyncerInfoRepository>().InstancePerLifetimeScope();

#if (DEBUG || STAGE)
            builder.RegisterType<ExtractFileFromFileSystemService>().As<IExtractFileService>().InstancePerLifetimeScope();
            builder.RegisterType<CryptoStreamerForFileSystem>().As<ICryptoStreamer>().InstancePerLifetimeScope();
#elif (RELEASE || PRODUCTION)
            builder.RegisterType<ExtractFileFromBlobStorageService>().As<IExtractFileService>().InstancePerLifetimeScope();
            builder.RegisterType<CryptoStreamerForBlobStorage>().As<ICryptoStreamer>().InstancePerLifetimeScope();
#endif
        }
    }
}