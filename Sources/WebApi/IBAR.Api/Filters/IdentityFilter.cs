using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using IBAR.TradeModel.Business.Services;

namespace IBAR.Api.Filters
{
    public class IdentityFilter : IAutofacActionFilter
    {
        private readonly IIdentityService _identityService;

        public IdentityFilter(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _identityService.SetIdentity(HttpContext.Current.User.Identity);
            return Task.FromResult(0);
        }
    }
}