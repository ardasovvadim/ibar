using IBAR.Api.Initialization;
using System;
using System.Web;
using System.Web.Http;

namespace IBAR.Api
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.DependencyResolver = AutofacConfig.GetResolver();
        }
    }
}