using System.Collections.Generic;
using IBAR.TradeModel.Business.ViewModels.Response;

namespace IBAR.Api.Data
{
    public class TotalDataParamQuery
    {
        public IEnumerable<int> DashboardTypes { get; set; }
        public PeriodString Period { get; set; }
        public IEnumerable<long> IdMasterAccounts { get; set; }
        public IEnumerable<long> IdTradeAccounts { get; set; }
        public TotalAccountListEnum Type { get; set; }
        public string SearchExpression { get; set; }
    }
}