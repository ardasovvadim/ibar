using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IBAR.Api.Filters
{
    public class AsyncActionAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            //var asyncHeader = actionContext.Request.Headers.FirstOrDefault(header => header.Key == "Async").Value
            //    .ToList()[0];
            //var uri = actionContext.Request.RequestUri.OriginalString;
            //if (asyncHeader != null)
            //{
            //    var asyncHeaderValue = bool.Parse(asyncHeader);
            //    if (asyncHeaderValue)
            //    {
            //        var connectionContext = GlobalHost.ConnectionManager.GetConnectionContext<RealtimeConnection>();
            //        var requestKey = Guid.NewGuid().ToString().Replace("-", "");
            //        var connectionId = actionContext.Request.Headers
            //            .FirstOrDefault(header => header.Key == "ConnectionId").Value.ToList()[0];
            //        HostingEnvironment.QueueBackgroundWorkItem(async (ct)
            //            =>
            //        {
            //            var response = await actionContext.ActionDescriptor.ExecuteAsync(
            //                actionContext.ControllerContext,
            //                actionContext.ActionArguments, ct);
            //            var data = new JavaScriptSerializer().Serialize(response);
            //            await connectionContext.Connection.Send(connectionId, $"{requestKey};{data}");
            //        });

            //        actionContext.Response = new HttpResponseMessage
            //        {
            //            StatusCode = HttpStatusCode.OK,
            //            Content = new StringContent(JsonConvert.SerializeObject(new {requestKey}))
            //        };
            //    }
            //}

            await Task.FromResult(0);
        }
    }
}