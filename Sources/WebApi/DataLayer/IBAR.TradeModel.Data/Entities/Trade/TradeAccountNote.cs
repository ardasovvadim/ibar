using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeAccountNote : EntityBase
    {
        public string Text { get; set; }
        public int NoteType { get; set; }

        [ForeignKey("CreatedById")]
        [InverseProperty("CreatedTradeAccountNotes")]
        public virtual User CreatedBy { get; set; }
        public long CreatedById { get; set; }
        
        [ForeignKey("TradeAccountId")] 
        public virtual TradeAccount TradeAccount { get; set; }
        public long TradeAccountId { get; set; }
    }
}