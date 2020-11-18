using System;
using System.Web.Http;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Request;

namespace IBAR.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/exception")]
    public class ExceptionController : ApiController
    {
        private readonly IExceptionService _exceptionService;

        public ExceptionController(IExceptionService exceptionService)
        {
            _exceptionService = exceptionService;
        }

        [HttpGet]
        [Route("GetException/{id}")]
        public string GetException(string id = "")
        {
            return _exceptionService.GetException(id);
        }

        [HttpPost]
        [Route("report/send")]
        public IHttpActionResult SendBugReport(ReportBugKey key)
        {
            _exceptionService.SendBugReport(key.Guid);
            return Ok();
        }
        
    }
}