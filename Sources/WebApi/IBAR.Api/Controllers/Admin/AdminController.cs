using System.Web.Http;
using IBAR.Api.Filters;
using IBAR.Api.Models;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IMasterAccountService _masterAccountService;

        public AdminController(
            IAuthenticationService authenticationService,
            IUserService userService,
            IMasterAccountService masterAccountService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _masterAccountService = masterAccountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("user/login")]
        [ValidateModel]
        public IHttpActionResult Login(LoginModel userModel)
        {
            return Ok(_authenticationService.LoginAs(userModel, "admin"));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("user/confirm/login")]
        [ValidateModel]
        public IHttpActionResult ConfirmLogin(ConfirmModel userModel)
        {
            return Ok(_authenticationService.ConfirmLogin(userModel, "admin"));
        }

        [HttpGet]
        [Route("user/all")]
        public IHttpActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpPut]
        [Route("user/update/role")]
        [ValidateModel]
        public IHttpActionResult ChangeRole(UserRoleModel modelParam)
        {
            return Ok(_userService.ChangeRole(modelParam));
        }

        [HttpPost]
        [Route("user/add")]
        [ValidateModel]
        public IHttpActionResult AddNewUser(UserCreateEditModel modelParam)
        {
            var result = _userService.AddNewUser(modelParam);
            return Ok(result);
        }

        [HttpDelete]
        [Route("user/delete/{id:long}")]
        public IHttpActionResult DeleteUser(long id)
        {
            return Ok(_userService.DeleteUser(id));
        }

        [HttpPut]
        [Route("user/update")]
        [ValidateModel]
        public IHttpActionResult UpdateUser(UserCreateEditModel modelParam)
        {
            return Ok(_userService.UpdateUser(modelParam));
        }

        [HttpGet]
        [Route("accounts/master/all")]
        public IHttpActionResult GetAllMasterAccounts()
        {
            return Ok(_masterAccountService.GetAllMasterAccountGridVm());
        }

        [HttpPost]
        [Route("accounts/master/add")]
        [ValidateModel]
        public IHttpActionResult AddNewAccountMaster(MasterAccountCreateEditModel modelParam)
        {
            return Ok(_masterAccountService.AddMasterAccount(modelParam));
        }

        [HttpDelete]
        [Route("accounts/master/delete/{id}")]
        public void DeleteMasterAccount(long id)
        {
            _masterAccountService.DeleteMasterAccount(id);
        }

        [HttpPut]
        [Route("accounts/master/update")]
        [ValidateModel]
        public IHttpActionResult UpdateMasterAccount(MasterAccountCreateEditModel modelParam)
        {
            return Ok(_masterAccountService.UpdateMasterAccount(modelParam));
        }

        [HttpPut]
        [Route("user/{id}/invite")]
        public void ResendInvite(long id)
        {
            _userService.ResendInvite(id);
        }
    }
}