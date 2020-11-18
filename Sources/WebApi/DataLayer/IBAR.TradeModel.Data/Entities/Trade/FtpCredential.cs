using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class FtpCredential : EntityBase
    {
        public FtpCredential()
        {
            MasterAccounts = new HashSet<MasterAccount>();
            ImportedFiles = new HashSet<ImportedFile>();
        }
        [MaxLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string FtpName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Url { get; set; }

        public virtual ICollection<MasterAccount> MasterAccounts { get; set; }
        public virtual ICollection<ImportedFile> ImportedFiles { get; set; }
    }
}