using System.Collections.Generic;

namespace IBAR.TradeModel.Business.ViewModels.Request.PortfolioPage
{
    public class PortfolioChartDataQueryParam
    {
        public string AccountName { get; set; }
        public IEnumerable<PeriodString> Periods { get; set; }
        public PortfolioChartType ChartType { get; set; }
    }
}