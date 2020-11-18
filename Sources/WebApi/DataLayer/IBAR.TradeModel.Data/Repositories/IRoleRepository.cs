using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll();
        Role GetDefault();
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly TradeModelContext _dbContext;

        public RoleRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Role> GetAll()
        {
            return _dbContext.Roles.Where(r => !r.Deleted).ToList();
        }

        public Role GetDefault()
        {
            return _dbContext.Roles.FirstOrDefault(r => r.Name == "user");
        }
    }
}