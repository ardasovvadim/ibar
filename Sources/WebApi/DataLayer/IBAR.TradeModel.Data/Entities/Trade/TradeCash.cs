using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeCash : EntityBase
    {
        public decimal Deposits { get; set; }
        public decimal Withdrawals { get; set; }
        public DateTime ReportDate { get; set; }

        [ForeignKey("TradeAccountId")]
        public virtual TradeAccount TradeAccount { get; set; }
        public long TradeAccountId { get; set; }
        [ForeignKey("ImportedFileId")]
        public virtual ImportedFile ImportedFile { get; set; }
        public long ImportedFileId { get; set; }
    }
}