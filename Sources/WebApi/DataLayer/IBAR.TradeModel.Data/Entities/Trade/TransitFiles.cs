using System.ComponentModel.DataAnnotations;

namespace IBAR.TradeModel.Data.Entities
{
    public class TransitFiles
    {
        [Key] public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string AccountName { get; set; }
    }
}