using IBAR.TradeModel.Data.Entities.Trade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBAR.TradeModel.Data.Entities
{
    public class TradeAccount : EntityBase
    {
        public TradeAccount()
        {
            TradeFees = new HashSet<TradeFee>();
            TradeNav = new HashSet<TradeNav>();
            TradeCashes = new HashSet<TradeCash>();
            TradeTradesAs = new HashSet<TradeTradesAs>();
            TradingPermissions = new HashSet<TradingPermission>();
            TradeAccountNotes = new HashSet<TradeAccountNote>();
            TradeSytossOpenPositions = new HashSet<TradeSytossOpenPosition>();
            TradesExes = new HashSet<TradesExe>();
            TradeInterestAccruas = new HashSet<TradeInterestAccrua>();
            TradeCommissions = new HashSet<TradeCommissions>();
        }
        [StringLength(10)]
        [Required(AllowEmptyStrings = false)]
        [Index(IsUnique = true)]
        public string AccountName { get; set; }
        public string AccountAlias { get; set; }
        public DateTime? DateFunded { get; set; }
        public DateTime? DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string СountryResidentialAddress { get; set; }
        public string CityResidentialAddress { get; set; }
        public string StreetResidentialAddress { get; set; }
        public string StateResidentialAddress { get; set; }
        public string IbEntity { get; set; }
        public string AccountType { get; set; }
        public string PostalCode { get; set; }
        public string PrimaryEmail { get; set; }
        public string Currency { get; set; }
        public string AccountCapabilities { get; set; }
        public string CustomerType { get; set; }
        public bool IsClientInfo { get; set; }
        [ForeignKey("MasterAccountId")]
        public virtual MasterAccount MasterAccount { get; set; }
        public long MasterAccountId { get; set; }
        public string TradeStatus { get; set; }
        public TradeRank TradeRank { get; set; }
        
        public virtual ICollection<TradeFee> TradeFees { get; set; }
        public virtual ICollection<TradeNav> TradeNav { get; set; }
        public virtual ICollection<TradeCash> TradeCashes { get; set; }
        public virtual ICollection<TradeTradesAs> TradeTradesAs { get; set; }
        public virtual ICollection<TradingPermission> TradingPermissions { get; set; }
        public virtual ICollection<TradeAccountNote> TradeAccountNotes { get; set; }

        public virtual ICollection<TradeSytossOpenPosition> TradeSytossOpenPositions { get; set; }
        public virtual ICollection<TradesExe> TradesExes { get; set; }
        public virtual ICollection<TradeInterestAccrua> TradeInterestAccruas { get; set; }
        public virtual ICollection<TradeCommissions> TradeCommissions { get; set; }
        
        [ForeignKey("ImportedFileId")]
        public virtual ImportedFile ImportedFile { get; set; }
        public long ImportedFileId { get; set; }
    }
}