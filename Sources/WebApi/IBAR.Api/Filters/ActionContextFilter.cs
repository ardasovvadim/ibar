using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using IBAR.TradeModel.Business.Services;

namespace IBAR.Api.Filters
{
    public class ActionContextFilter : IAutofacActionFilter
    {
        private readonly IActionContextAccessor _actionContextAccessor;

        public ActionContextFilter(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }
        
        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _actionContextAccessor.SetContext(actionContext);
            return Task.FromResult(0);
        }
    }
}