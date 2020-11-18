using Autofac.Integration.WebApi;
using IBAR.TradeModel.Business.Common.Log;
using System;
using System.Data.Entity.ModelConfiguration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace IBAR.Api.Filters
{
    // use for Logging all exceptions
    public class ExceptionHandlerFilter : IAutofacExceptionFilter
    {
        public IApiLogger ApiLogger { get; set; }

        public Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var errorGuild = Guid.NewGuid();
            ApiLogger.LogError(GetErrorLogMessage(actionExecutedContext, errorGuild), errorGuild, actionExecutedContext.Exception);

            if (actionExecutedContext.Exception is ModelValidationException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionExecutedContext.ActionContext.ModelState);
                return Task.CompletedTask;
            }

            if (actionExecutedContext.Exception is FileNotFoundException) {

                var fileNotFoundException = new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    RequestMessage = actionExecutedContext.Request,
                    Content = new StringContent(actionExecutedContext.Exception.ToString())

                });
                throw fileNotFoundException;
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                RequestMessage = actionExecutedContext.Request,
                Content = new StringContent(errorGuild.ToString())
            });
        }

        private string GetErrorLogMessage(HttpActionExecutedContext context, Guid errorGuild)
        {
            var result = new StringBuilder();
            result.AppendLine($"GUID: {errorGuild}");
            result.AppendLine($"URI: {context.Request.RequestUri.PathAndQuery}");
            result.AppendLine($"CONTROLLER NAME: {context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName}");
            result.AppendLine($"ACTION NAME: {context.ActionContext.ActionDescriptor.ActionName}");
            result.AppendLine("PARAMS:");
            foreach (var parameterItem in context.ActionContext.ActionArguments)
            {
                result.AppendLine($"PARAM NAME: {parameterItem.Key} | PARAM VALUE: {parameterItem.Value}");
            }

            TraceException(result, context.Exception);

            return result.ToString();
        }

        // TODO: Should move this extensions to Core project with common logic
        private static void TraceException(StringBuilder trace, Exception ex, bool inner = false)
        {
            trace.AppendLine(inner ? "INNEREXCEPTION" : "EXCEPTION");
            trace.AppendFormat("MESSAGE: {0} {1}", ex.Message,
                ex.Message.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine);
            trace.AppendFormat("STACKTRACE: {0} {1}", ex.StackTrace,
                ex.StackTrace.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine);

            if (ex.InnerException != null)
                TraceException(trace, ex.InnerException, true);

            trace.AppendLine();
        }
    }
}