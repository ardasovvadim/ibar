using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradingPermission
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public ICollection<TradeAccount> TradeAccounts { get; set; }
    }
}