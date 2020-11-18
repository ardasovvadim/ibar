using IBAR.TradeModel.Data.Entities.Trade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class ImportedFile : EntityBase
    {
        [MaxLength(250)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string OriginalFileName { get; set; }
        public DateTime FileCreateDate { get; set; }
        public FileState FileState { get; set; }
        public FileStatus FileStatus { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime? CopiedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
        [ForeignKey("MasterAccountId")]
        public virtual MasterAccount MasterAccount { get; set; }
        public long MasterAccountId { get; set; }

        [ForeignKey("FtpCredentialId")]
        public virtual FtpCredential FtpCredential { get; set; }
        public long? FtpCredentialId { get; set; }

        public virtual FileUpload FileUpload { get; set; }
        
        public virtual ICollection<TradeAccount> TradeAccounts { get; set; } = new HashSet<TradeAccount>();
        public virtual ICollection<TradeCash> TradeCashes { get; set; } = new HashSet<TradeCash>();
        public virtual ICollection<TradeFee> TradeFees { get; set; } = new HashSet<TradeFee>();
        public virtual ICollection<TradeNav> TradeNavs { get; set; } = new HashSet<TradeNav>();
        public virtual ICollection<TradeTradesAs> TradesAsses { get; set; } = new HashSet<TradeTradesAs>();
        public virtual ICollection<TradeCommissions> TradeCommissions { get; set; } = new HashSet<TradeCommissions>();
        public virtual ICollection<TradeSytossOpenPosition> SytossOpenPositions { get; set; } = new HashSet<TradeSytossOpenPosition>();
        public virtual ICollection<TradesExe> TradesExes { get; set; } = new HashSet<TradesExe>();
        public virtual ICollection<TradeInterestAccrua> TradeInterestAccruas { get; set; } = new HashSet<TradeInterestAccrua>();
    }
}