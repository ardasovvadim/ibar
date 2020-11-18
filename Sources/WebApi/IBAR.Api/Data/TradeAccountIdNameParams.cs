using System.Collections.Generic;

namespace IBAR.Api.Data
{
    public class TradeAccountIdNameParams
    {
        public int PageIndex { get; set; }
        public int PageLength { get; set; }
        public PeriodString Period { get; set; }
        public string SortBy { get; set; }
        public IEnumerable<long> MasterAccounts { get; set; }
        public IEnumerable<long> TradeAccounts { get; set; }
    }
}