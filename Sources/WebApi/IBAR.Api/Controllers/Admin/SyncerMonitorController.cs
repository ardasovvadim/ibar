using System.Web.Http;
using IBAR.TradeModel.Business.Services.Wcf;

namespace IBAR.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/monitor")]
    public class SyncerMonitorController : ApiController
    {
        private readonly IWcfService _wcfService;

        public SyncerMonitorController(IWcfService wcfService)
        {
            _wcfService = wcfService;
        }

        [HttpGet]
        [Route("reload/{param}")]
        public void ReloadJob(string param)
        {
            _wcfService.ReloadJob(param);
        }

        [HttpGet]
        [Route("start/{param}")]
        public void StartJob(string param)
        {
            _wcfService.StartJob(param);
        }

        [HttpGet]
        [Route("stop/{param}")]
        public void StopJob(string param)
        {
            _wcfService.StopJob(param);
        }

        [HttpGet]
        [Route("status/{param}")]
        public IHttpActionResult StatusJob(string param)
        {
            _wcfService.StatusJob(param);
            return Ok();
        }
    }
}