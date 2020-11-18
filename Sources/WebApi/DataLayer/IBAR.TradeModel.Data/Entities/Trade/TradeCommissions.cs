using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Data.Entities.Trade
{
    public class TradeCommissions : EntityBase
    {
        public DateTime ReportDate { get; set; }
        public decimal FxRateToBase { get; set; }
        public decimal TotalCommission { get; set; }

        [ForeignKey("TradeAccountId")]
        public virtual TradeAccount TradeAccount { get; set; }
        public long TradeAccountId { get; set; }
        [ForeignKey("ImportedFileId")]
        public virtual ImportedFile ImportedFile { get; set; }
        public long ImportedFileId { get; set; }
    }
}
