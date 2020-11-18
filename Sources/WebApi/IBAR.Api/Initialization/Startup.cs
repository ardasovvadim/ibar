using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Cors;
using IBAR.Api.Initialization;
using IBAR.Api.SignalR;
using IBAR.TradeModel.Business.SignalR;
using IBAR.TradeModel.Business.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace IBAR.Api.Initialization
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // app.UseCors(CorsOptions.AllowAll);
            
            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = true,
                SupportsCredentials = true,
                ExposedHeaders = { "FileName"}
            };
            
            app.UseCors(new CorsOptions
            {
                PolicyProvider= new CorsPolicyProvider
                {
                    PolicyResolver = c => Task.FromResult(policy)
                }
            });
            
            ConfigureOAuth(app);
            app.MapSignalR<RealtimeConnection>("/signalr");
            app.MapSignalR<LogConnection>("/log");
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            var appDomain = ConfigurationManager.AppSettings["appDomain"];
            var jwtSecret = ConfigurationManager.AppSettings["jwtSecret"];
            var key = jwtSecret.ToSymmetricSecurityKey();
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidIssuer = appDomain,
                    ValidAudience = appDomain,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }
            });
        }
    }
}