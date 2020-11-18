using IBAR.Api.Common.Extensions;
using IBAR.Api.Data;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Dashboard;
using System.Collections.Generic;
using System.Web.Http;

namespace IBAR.Api.Controllers.Reporting
{
    [Authorize]
    [RoutePrefix("api/total-accounts")]
    public class TotalAccountsController : ApiController
    {
        private readonly ITotalAccountService _totalAccountService;

        public TotalAccountsController(ITotalAccountService totalAccountService)
        {
            _totalAccountService = totalAccountService;
        }


        [HttpPost]
        [Route("data/get-table")]
        public List<TotalAccountTableModel> GetTableData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _totalAccountService.GetTableData(totalDataParamQuery.IdMasterAccounts,
                totalDataParamQuery.IdTradeAccounts);
        }


        [HttpPost]
        [Route("data/get-list")]
        public List<TotalAccountListModel> GetListData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _totalAccountService.GetListData(
                totalDataParamQuery.Period.ToPeriod(),
                totalDataParamQuery.IdMasterAccounts,
                totalDataParamQuery.IdTradeAccounts);
        }


        [HttpPost]
        [Route("search")]
        public TotalAccountListModel FilterData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _totalAccountService.FilterListData(totalDataParamQuery.Type, 
                totalDataParamQuery.SearchExpression, 
                totalDataParamQuery.IdMasterAccounts,
                totalDataParamQuery.IdTradeAccounts);
        }


        [HttpPost]
        [Route("data/get-totals")]
        public TotalDataVm<TotalAccountEnum> GetTotalData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _totalAccountService.GetTotals(totalDataParamQuery.DashboardTypes.ToDashboardEnumTypes<TotalAccountEnum>(),
                totalDataParamQuery.Period.ToPeriod(),
                totalDataParamQuery.IdMasterAccounts,
                totalDataParamQuery.IdTradeAccounts);
        }
    }
}
