using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IInviteRepository
    {
        Invite GetByLinkKey(string linkKey);
        void Save(Invite invite);
        Invite Add(Invite invite);
        IQueryable<Invite> Query();
    }

    public class InviteRepository : IInviteRepository
    {
        private readonly TradeModelContext _dbContext;

        public InviteRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Invite GetByLinkKey(string linkKey)
        {
            return _dbContext.Invites.FirstOrDefault(inv => inv.LinkKey == linkKey);
        }

        public void Save(Invite invite)
        {
            _dbContext.Entry(invite).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public Invite Add(Invite invite)
        {
            var dto = _dbContext.Invites.Add(invite);
            _dbContext.SaveChanges();
            return dto;
        }

        public IQueryable<Invite> Query()
        {
            return _dbContext.Invites;
        }
    }
}