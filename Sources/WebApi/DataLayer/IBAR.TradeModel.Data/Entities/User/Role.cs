using System.Collections.Generic;

namespace IBAR.TradeModel.Data.Entities
{
    public class Role : EntityBase
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}