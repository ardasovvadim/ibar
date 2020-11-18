using IBAR.Api.Common.Extensions;
using IBAR.Api.Data;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IBAR.Api.Controllers.Reporting
{
    [Authorize]
    [RoutePrefix("api/income")]
    public class IncomeController : ApiController
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost]
        [Route("total-income")]
        public ChartDataVm LoadTotalIncome([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _incomeService.LoadTotalIncome(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts, chartDataParamQuery.IdTradeAccounts);
        }

        [HttpPost]
        [Route("trade-commissions")]
        public ChartDataVm LoadTradingCommissions([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _incomeService.LoadTradingCommissions(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts, chartDataParamQuery.IdTradeAccounts);
        }

        [HttpPost]
        [Route("trade-interest")]
        public ChartDataVm LoadInterest([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _incomeService.LoadInterest(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts, chartDataParamQuery.IdTradeAccounts);
        }

        [HttpPost]
        [Route("trade-borrowing")]
        public ChartDataVm LoadBorrowing([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _incomeService.LoadBorrowing(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts, chartDataParamQuery.IdTradeAccounts);
        }

        [HttpPost]
        [Route("data/get-table")]
        public List<TotalAccountTableModel> GetTableData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _incomeService.GetTableData(totalDataParamQuery.IdMasterAccounts,
                totalDataParamQuery.IdTradeAccounts);
        }
    }
}
