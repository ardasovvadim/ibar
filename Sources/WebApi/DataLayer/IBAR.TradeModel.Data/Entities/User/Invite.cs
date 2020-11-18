using System;

namespace IBAR.TradeModel.Data.Entities
{
    public class Invite : EntityBase
    {
        public string LinkKey { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long IdUser { get; set; }
        public bool IsUsed { get; set; }
        public int PhoneCode { get; set; }
        public bool IsConfirmPhoneCode { get; set; }
    }
}