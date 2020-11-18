using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeInstrument : EntityBase
    {
        public TradeInstrument()
        {
            TradeFees = new HashSet<TradeFee>();
        }
        [MaxLength(5)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string InstrumentName { get; set; }
        public virtual ICollection<TradeFee> TradeFees { get; set; }
    }
}