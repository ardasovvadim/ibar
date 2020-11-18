using System.Collections.Generic;

namespace IBAR.TradeModel.Business.ViewModels.Request.Admin
{
    public class AnalyticsChartDataParams
    {
        public long MasterAccountId { get; set; }
        public long FtpCredentialId { get; set; }
        public IEnumerable<PeriodString> Periods { get; set; }
    }
}