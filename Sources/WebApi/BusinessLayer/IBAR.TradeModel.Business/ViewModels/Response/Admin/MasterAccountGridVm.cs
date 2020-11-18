using System;

namespace IBAR.TradeModel.Business.ViewModels.Response.Admin
{
    public class MasterAccountGridVm
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string AccountAlias { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool Deleted { get; set; }
        public int AmountTradeAccounts { get; set; }
    }
}