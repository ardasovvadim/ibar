using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Business.ViewModels.Request.ClientsPage
{
    public class TradeAccountRankEditModel
    {
        public long Id { get; set; }
        public TradeRank TradeRank { get; set; }
    }
}