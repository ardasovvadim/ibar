using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class FileNameRegex
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string FileName { get; set; }
        public string FileRegex { get; set; }
    }
}
