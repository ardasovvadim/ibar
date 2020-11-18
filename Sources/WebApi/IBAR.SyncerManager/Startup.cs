//using System;
//using IBAR.SyncerManager;
//using Microsoft.Owin;
//using Microsoft.Owin.Cors;
//using Owin;

//[assembly: OwinStartup(typeof(Startup))]
//namespace IBAR.SyncerManager
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            app.UseCors(CorsOptions.AllowAll);
//            app.MapSignalR<LogConnection>("/log");
//            Console.WriteLine("SignalR is running");
//        }
//    }
//}