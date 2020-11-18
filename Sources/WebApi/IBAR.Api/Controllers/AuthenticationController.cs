using System.Web.Http;
using IBAR.Api.Filters;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/auth")]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost]
        [Route("Login")]
        [ValidateModel]
        public IHttpActionResult Login(LoginModel userModel)
        {
            return Ok(_authenticationService.LoginAs(userModel, "user"));
        }

        [HttpPost]
        [Route("ConfirmLogin")]
        [ValidateModel]
        public IHttpActionResult ConfirmLogin(ConfirmModel userModel)
        {
            return Ok(_authenticationService.ConfirmLogin(userModel, "user"));
        }
    }
}