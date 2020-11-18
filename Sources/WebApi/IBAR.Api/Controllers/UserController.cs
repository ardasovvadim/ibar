using System.Web.Http;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Request;

namespace IBAR.Api.Controllers
{
    [Authorize(Roles = "user")]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private IUserService UserService { get; }
        
        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpPut]
        [Route("change-password")]
        public void ChangePassword(ChangePasswordEditModel model)
        {
            UserService.ChangePassword(model);
        }
    }
}