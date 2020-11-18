using System;
using System.Web.Http;
using IBAR.Api.Common.Extensions;
using IBAR.Api.Data;
using IBAR.TradeModel.Business.Data;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Dashboard;

namespace IBAR.Api.Controllers.Reporting
{
    [Authorize]
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        
        [HttpPost]
        [Route("total-income")]
        public ChartDataVm LoadTotalIncome([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _dashboardService.LoadTotalIncome(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts, chartDataParamQuery.IdTradeAccounts);
        }
        
        [HttpPost]
        [Route("total-clients")]
        public ChartDataVm LoadTotalClients([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _dashboardService.LoadTotalClients(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts , chartDataParamQuery.IdTradeAccounts);
        }
        
        [HttpPost]
        [Route("total-aum")]
        public ChartDataVm LoadTotalAum([FromBody] ChartDataParamQuery chartDataParamQuery)
        {
            return _dashboardService.LoadTotalAum(chartDataParamQuery.Periods.ToPeriodList(), chartDataParamQuery.IdMasterAccounts , chartDataParamQuery.IdTradeAccounts);
        }

        [HttpPost]
        [Route("GetTotals")]
        public TotalDataVm<DashboardEnum> GetTotalData([FromBody] TotalDataParamQuery totalDataParamQuery)
        {
            return _dashboardService.GetTotals(totalDataParamQuery.DashboardTypes.ToDashboardEnumTypes<DashboardEnum>(), 
                totalDataParamQuery.Period.ToPeriod(), 
                totalDataParamQuery.IdMasterAccounts, 
                totalDataParamQuery.IdTradeAccounts);
        }
    }
}