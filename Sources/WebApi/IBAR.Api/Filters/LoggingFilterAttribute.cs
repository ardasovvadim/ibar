using Autofac.Integration.WebApi;
using IBAR.TradeModel.Business.Common.Log;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace IBAR.Api.Filters
{
    public class LoggingFilterAttribute : IAutofacActionFilter
    {
        public IApiLogger ApiLogger { get; set; }

        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            ApiLogger.LogInfo(GetLogMessage(actionContext));
            return Task.CompletedTask;
        }

        private string GetLogMessage(HttpActionContext actionContext)
        {
            var result = new StringBuilder();
            result.AppendLine($"URI: {actionContext.Request.RequestUri.PathAndQuery}");
            result.AppendLine($"CONTROLLER NAME: {actionContext.ControllerContext.ControllerDescriptor.ControllerName}");
            result.AppendLine($"ACTION NAME: {actionContext.ActionDescriptor.ActionName}");
            result.AppendLine("PARAMS:");
            foreach (var parameterItem in actionContext.ActionArguments)
            {
                result.AppendLine($"PARAM NAME: {parameterItem.Key} | PARAM VALUE: {parameterItem.Value}");
            }
            return result.ToString();
        }
    }
}