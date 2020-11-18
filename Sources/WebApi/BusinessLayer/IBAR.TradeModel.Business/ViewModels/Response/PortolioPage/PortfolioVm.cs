namespace IBAR.TradeModel.Business.ViewModels.Response.PortolioPage
{
    public class PortfolioVm
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        
        public decimal Total { get; set; }
        public decimal Cash { get; set; }
        public decimal Stock { get; set; }
        public decimal Options { get; set; }
        public decimal Commodities { get; set; }
        public decimal InterestAccruals { get; set; }
        
        public decimal TotalLong { get; set; }
        public decimal CashLong { get; set; }
        public decimal StockLong { get; set; }
        public decimal OptionsLong { get; set; }
        public decimal CommoditiesLong { get; set; }
        public decimal InterestAccrualsLong { get; set; }
        
        public decimal TotalShort { get; set; }
        public decimal CashShort { get; set; }
        public decimal StockShort { get; set; }
        public decimal OptionsShort { get; set; }
        public decimal CommoditiesShort { get; set; }
        public decimal InterestAccrualsShort { get; set; }
    }
}