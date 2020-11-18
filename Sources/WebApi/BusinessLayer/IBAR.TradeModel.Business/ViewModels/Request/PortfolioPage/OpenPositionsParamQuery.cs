namespace IBAR.TradeModel.Business.ViewModels.Request.PortfolioPage
{
    public class OpenPositionsParamQuery
    {
        public string DateString { get; set; }
        public string AccountName { get; set; }
        public int PageLength { get; set; }
        public int PageIndex { get; set; }
    }
}