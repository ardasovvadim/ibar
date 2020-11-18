using System.Collections.Generic;
using System.Web.Http;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Request.Admin;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/ftp")]
    public class FtpController : ApiController
    {
        private readonly IFtpCredentialService _ftpCredentialService;

        public FtpController(IFtpCredentialService ftpCredentialService)
        {
            _ftpCredentialService = ftpCredentialService;
        }

        [HttpGet]
        [Route("GetAllFtpCredentials")]
        public IHttpActionResult GetAllFtpCredentials()
        {
            return Ok(_ftpCredentialService.GetAll());
        }

        [HttpPost]
        [Route("AddNewFtpCredential")]
        public IHttpActionResult AddNewFtpCredential(FtpCredentialCreateEditModel model)
        {
            return Ok(_ftpCredentialService.AddNewFtpCredential(model));
        }

        [HttpDelete]
        [Route("DeleteFtpCredential/{id}")]
        public IHttpActionResult DeleteFtpCredential(long id)
        {
            _ftpCredentialService.DeleteFtpCredential(id);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateFtpCredential")]
        public IHttpActionResult UpdateFtpCredential(FtpCredentialCreateEditModel model)
        {
            return Ok(_ftpCredentialService.UpdateFtpCredential(model));
        }

        [HttpGet]
        [Route("id-name")]
        public IEnumerable<IdNameModel> GetIdNames()
        {
            return _ftpCredentialService.GetIdNames();
        }
    }
}