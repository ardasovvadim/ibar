using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response.ClientsPage
{
    public class TradesExeInformationVm
    {
        public decimal? Quantity { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public long? VolatilityOrderLink { get; set; }
        public string AssetCategory { get; set; }
        public string PutCall { get; set; }
        public string BuySell { get; set; }
        public string Description { get; set; }
        public string OrderType { get; set; }
        public string OrderReference { get; set; }
        public string Currency { get; set; }
        public string IbExecId { get; set; }
        public string IsAPIOrder { get; set; }
        public string OpenCloseIndicator { get; set; }
        public string Isin { get; set; }
        public decimal? Multiplier { get; set; }
        public decimal? FxRateToBase { get; set; }
        public long? TransactionID { get; set; }
        public long? IbOrderID { get; set; }
        public decimal? IbCommission { get; set; }
        public decimal? Conid { get; set; }
        public decimal? Strike { get; set; }
        public decimal? Taxes { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime? SettleDateTarget { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? ReportDate { get; set; }
        public string Symbol { get; set; }
        public string ListingExchange { get; set; }
        public string UnderlyingSymbol { get; set; }
    }
}
