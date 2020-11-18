using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IMasterAccountRepository
    {
        MasterAccount GetById(long id);
        IEnumerable<MasterAccount> GetAll();
        MasterAccount Add(MasterAccount dto);
        bool IsExists(MasterAccount dto);
        MasterAccount Update(MasterAccount account);
        IQueryable<MasterAccount> Query();
        MasterAccount GetEntry(MasterAccount dto);
    }

    public class MasterAccountRepository : IMasterAccountRepository
    {
        private readonly TradeModelContext _dbContext;

        public MasterAccountRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public MasterAccount GetById(long id)
        {
            return _dbContext.MasterAccounts.FirstOrDefault(acc => acc.Id == id && !acc.Deleted);
        }

        public IEnumerable<MasterAccount> GetAll()
        {
            return _dbContext.MasterAccounts.Where(acc => !acc.Deleted);
        }

        public MasterAccount Add(MasterAccount dto)
        {
            dto = _dbContext.MasterAccounts.Add(dto);
            _dbContext.SaveChanges();
            return dto;
        }

        public bool IsExists(MasterAccount dto)
        {
            return _dbContext
                    .MasterAccounts
                    .Any(acc => (acc.AccountName == dto.AccountName ||
                                (acc.AccountAlias != null && dto.AccountAlias != null && acc.AccountAlias == dto.AccountAlias) ||
                                acc.Id == dto.Id) && !acc.Deleted);
        }

        public MasterAccount Update(MasterAccount account)
        {
            _dbContext.Entry(account).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return account;
        }

        public IQueryable<MasterAccount> Query()
        {
            return _dbContext.MasterAccounts.Where(acc => !acc.Deleted);
        }

        public MasterAccount GetEntry(MasterAccount dto)
        {
            return _dbContext
                    .MasterAccounts
                    .FirstOrDefault(acc => (acc.AccountName == dto.AccountName ||
                                           (acc.AccountAlias != null && dto.AccountAlias != null && acc.AccountAlias == dto.AccountAlias) ||
                                           acc.Id == dto.Id) && !acc.Deleted);
        }
    }
}