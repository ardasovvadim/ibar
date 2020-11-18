using System.Collections.Generic;
using System.Web.Http;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Controllers.Reporting
{
    // [AsyncAction]
    [Authorize]
    [RoutePrefix("api/tradeaccount")]
    public class TradeAccountController : ApiController
    {
        private readonly ITradeAccountService _tradeAccountService;

        public TradeAccountController(ITradeAccountService tradeAccountService)
        {
            _tradeAccountService = tradeAccountService;
        }

        [HttpPost]
        [Route("")]
        public IEnumerable<TradeAccountModel> GetAll()
        {
            return _tradeAccountService.GetAllTradeAccounts();
        }

        [HttpGet]
        [Route("GetBySearchName/{searchName}")]
        public TradeAccountModel GetBySearchName(string searchName)
        {
            return _tradeAccountService.GetBySearchName(searchName);
        }
    }
}