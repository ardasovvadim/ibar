using System;

namespace IBAR.TradeModel.Business.ViewModels.Response.PortolioPage
{
    public class OpenPositionVm
    {
        public string Symbol { get; set; }
        public decimal Position { get; set; }
        public decimal MarkPrice { get; set; }
        public decimal CostBasisPrice { get; set; }
        public decimal CostBasisMoney { get; set; }
        public decimal PercentOfNav { get; set; }
        public decimal FifoPnlUnrealized { get; set; }
        public string Description { get; set; }
        public string AssetCategory { get; set; }
        public string UnderlyingListingExchange { get; set; }
        public string Currency { get; set; }
        public string Isin { get; set; }
        public decimal Conid { get; set; }
        public decimal FxRateToBase { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string PutCall { get; set; }
        public decimal Multiplier { get; set; }
        public decimal Strike { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime ReportDate { get; set; }
    }
}