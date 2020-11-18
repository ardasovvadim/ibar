using System.Web.Http;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Request.Admin;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Admin;

namespace IBAR.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/analytics")]
    public class AnalyticsController : ApiController
    {
        private IAnalyticsService AnalyticsService { get; }
        
        public AnalyticsController(IAnalyticsService analyticsService)
        {
            AnalyticsService = analyticsService;
        }

        [HttpGet]
        [Route("{masterAccountId}/{ftpCredId}")]
        public AnalyticsCardInfoVm GetCardInfo(long masterAccountId = 0, long ftpCredId = 0)
        {
            return AnalyticsService.GetCardInfo(masterAccountId, ftpCredId);
        }

        [HttpPost]
        [Route("chart-data")]
        public ChartDataVm GetChartDate(AnalyticsChartDataParams @params)
        {
            return AnalyticsService.GetChartData(@params);
        }
    }
}