using System.Web.Http;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/registration")]
    public class RegistrationController : ApiController
    {
        private readonly IInviteService _inviteService;

        public RegistrationController(IInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpGet]
        [Route("IsWaitingConfirmation/{linkKey}")]
        public IHttpActionResult IsWaitingConfirmation(string linkKey)
        {
            return Ok(_inviteService.IsWaitingConfirmation(linkKey));
        }

        [HttpGet]
        [Route("SendRegistrationCode/{linkKey}")]
        public IHttpActionResult SendRegistrationCode(string linkKey)
        {
            _inviteService.SendRegistrationCode(linkKey);
            return Ok();
        }

        [HttpPost]
        [Route("ConfirmRegistration")]
        public IHttpActionResult ConfirmRegistration([FromBody] (string linkKey, int phoneCode) obj)
        {
            var (linkKey, phoneCode) = obj;
            return Ok(_inviteService.ConfirmRegistration(linkKey, phoneCode));
        }

        [HttpPost]
        [Route("FinishRegistration")]
        public IHttpActionResult FinishRegistration([FromBody] (string linkKey, ConfirmModel modelParam) obj)
        {
            var (linkKey, modelParam) = obj;
            _inviteService.FinishRegistration(linkKey, modelParam);
            return Ok();
        }
    }
}