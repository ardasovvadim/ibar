using System;

namespace IBAR.TradeModel.Business.ViewModels.Response.AccountInfoPage
{
    public class TradeAccountInfoGridViewModel
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string AccountAlias { get; set; }
        public string Name { get; set; }
        public DateTime? DateFunded { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
    }
}