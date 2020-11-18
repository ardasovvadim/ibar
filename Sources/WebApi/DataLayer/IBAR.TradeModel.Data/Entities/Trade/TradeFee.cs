using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeFee : EntityBase
    {
        public string ExternalTradeName { get; set; }
        public string ExternalExecName { get; set; }
        public DateTime ExternalDate { get; set; }
        public decimal RateToBase { get; set; }
        public decimal RevenueInCurrency { get; set; }
        public decimal ExpenseInCurrency { get; set; }
        public decimal NetInCurrency { get; set; }
        public decimal RevenueInBase { get; set; }
        public decimal ExpenseInBase { get; set; }
        public decimal NetInBase { get; set; }

        [ForeignKey("TradeAccountId")]
        public virtual TradeAccount TradeAccount { get; set; }
        public long TradeAccountId { get; set; }
        [ForeignKey("TradeFeeTypeId")]
        public virtual TradeFeeType TradeFeeType { get; set; }
        public long TradeFeeTypeId { get; set; }
        [ForeignKey("TradeInstrumentId")]
        public virtual TradeInstrument TradeInstrument { get; set; }
        public long TradeInstrumentId { get; set; }
        [ForeignKey("ImportedFileId")]
        public virtual ImportedFile ImportedFile { get; set; }
        public long ImportedFileId { get; set; }
    }
}