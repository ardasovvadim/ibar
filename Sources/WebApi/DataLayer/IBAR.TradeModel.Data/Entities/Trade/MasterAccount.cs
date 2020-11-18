using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class MasterAccount : EntityBase
    {
        public MasterAccount()
        {
            TradeFees = new HashSet<TradeFee>();
            TradeAccounts = new HashSet<TradeAccount>();
            ImportedFiles = new HashSet<ImportedFile>();
            FtpCredentials = new HashSet<FtpCredential>();
        }

        [MaxLength(10)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string AccountName { get; set; }
        [ForeignKey("UpdatedById")]
        [InverseProperty("UpdatedMasterAccounts")]
        public virtual User UpdatedBy { get; set; }
        public long? UpdatedById { get; set; }
        [ForeignKey("CreatedById")]
        [InverseProperty("CreatedMasterAccounts")]
        public virtual User CreatedBy { get; set; }
        public long? CreatedById { get; set; }
        public string AccountAlias { get; set; }

        public virtual ICollection<TradeFee> TradeFees { get; set; }
        public virtual ICollection<TradeAccount> TradeAccounts { get; set; }
        public virtual ICollection<ImportedFile> ImportedFiles { get; set; }
        public virtual ICollection<FtpCredential> FtpCredentials { get; set; }
    }
}