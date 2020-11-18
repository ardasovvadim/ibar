using System;

namespace IBAR.TradeModel.Data.Entities
{
    public class LogInfo : EntityBase
    {
        public DateTime? LoginTime { get; set; }
        public int VerificationCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}