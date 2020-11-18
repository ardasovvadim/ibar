using System.Collections.Generic;

namespace IBAR.TradeModel.Data.Entities
{
    public class User : EntityBase
    {
        public User()
        {
            Roles =  new HashSet<Role>();
            LogInfos = new HashSet<LogInfo>();
            CreatedTradeAccountNotes = new HashSet<TradeAccountNote>();
            CreatedMasterAccounts = new HashSet<MasterAccount>();
            UpdatedMasterAccounts = new HashSet<MasterAccount>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string AccessToken { get; set; }
        public bool IsWaitingConfirmation { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<LogInfo> LogInfos { get; set; }
        public virtual ICollection<TradeAccountNote> CreatedTradeAccountNotes  { get; set; }
        public virtual ICollection<MasterAccount> CreatedMasterAccounts  { get; set; }
        public virtual ICollection<MasterAccount> UpdatedMasterAccounts  { get; set; }
        
    }
}