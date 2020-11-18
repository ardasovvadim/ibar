using System.Collections.Generic;

namespace IBAR.Api.Data
{
    public class ChartDataParamQuery
    {
        public List<PeriodString> Periods { get; set; }
        public long[] IdMasterAccounts { get; set; }
        public long[] IdTradeAccounts { get; set; }
    }
}