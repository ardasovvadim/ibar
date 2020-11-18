using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities.Trade
{
    public class TradeInterestAccrua: EntityBase
    {
        public DateTime ReportDate { get; set; }
        public decimal EndingAccrualBalance { get; set; }

        [ForeignKey("TradeAccountId")]
        public virtual TradeAccount TradeAccount { get; set; }
        public long TradeAccountId { get; set; }
        
        [ForeignKey("ImportedFileId")]
        public virtual ImportedFile ImportedFile { get; set; }
        public long ImportedFileId { get; set; }
    }
}
