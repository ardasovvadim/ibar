using System.Collections.Generic;
using System.Web.Http;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers.Reporting
{
    [Authorize]
    [RoutePrefix("api/masteraccount")]
    public class MasterAccountController : ApiController
    {
        private readonly IMasterAccountService _masterAccountService;

        public MasterAccountController(IMasterAccountService masterAccountService)
        {
            _masterAccountService = masterAccountService;
        }

        [Route("GetAll")]
        [HttpGet]
        public IEnumerable<MasterAccountVm> GetAll()
        {
            return _masterAccountService.GetAll();
        }

        [HttpGet]
        [Route("id-name")]
        public IEnumerable<IdNameModel> GetIdNames()
        {
            return _masterAccountService.GetIdNames();
        }
    }
}