using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeFeeType : EntityBase
    {
        public TradeFeeType()
        {
            TradeFees = new HashSet<TradeFee>();
        }
        [MaxLength(25)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string TradeFeeTypeName { get; set; }

        public virtual ICollection<TradeFee> TradeFees { get; set; }
    }
}