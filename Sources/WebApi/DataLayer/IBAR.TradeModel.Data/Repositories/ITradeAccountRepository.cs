using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ITradeAccountRepository
    {
        IEnumerable<TradeAccount> GetAll();
        TradeAccount GetBySearchName(string searchName);
        IQueryable<TradeAccount> Query();
        IEnumerable<TradingPermission> GetAllTradingPermissions();
        bool IsExistsTradeAccount(long id);
        TradeAccountNote AddNewTradeAccountNote(TradeAccountNote dto);
        bool IsExistsTradeAccountNote(long id);
        void DeleteTradeAccountNote(long id);
        TradeAccount GetById(long id);
        TradeAccount ChangeTradeRank(TradeAccount account);
        IQueryable<TradeNav> QueryTradeNav();
        IQueryable<TradeSytossOpenPosition> QueryOpenPositions();
    }

    public class TradeAccountRepository : ITradeAccountRepository
    {
        private readonly TradeModelContext _dbContext;

        public TradeAccountRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TradeAccount> GetAll()
        {
            return _dbContext.TradeAccounts.Where(acc => !acc.Deleted);
        }

        public TradeAccount GetBySearchName(string searchName)
        {
            return _dbContext
                    .TradeAccounts
                    .FirstOrDefault(acc => !acc.Deleted && acc.AccountName.Trim() == searchName.Trim());
        }

        public IQueryable<TradeAccount> Query()
        {
            return _dbContext.Set<TradeAccount>();
        }

        public IEnumerable<TradingPermission> GetAllTradingPermissions()
        {
            return _dbContext.TradingPermissions;
        }

        public bool IsExistsTradeAccount(long id)
        {
            return _dbContext.TradeAccounts.Any(acc => !acc.Deleted && acc.Id == id);
        }

        public TradeAccountNote AddNewTradeAccountNote(TradeAccountNote dto)
        {
            dto = _dbContext.TradeAccountNotes.Add(dto);
            _dbContext.SaveChanges();
            return dto;
        }

        public bool IsExistsTradeAccountNote(long id)
        {
            return _dbContext.TradeAccountNotes.Any(note => !note.Deleted && note.Id == id);
        }

        public void DeleteTradeAccountNote(long id)
        {
            var dto =_dbContext.TradeAccountNotes.FirstOrDefault(note => !note.Deleted && note.Id == id);
            _dbContext.TradeAccountNotes.Remove(dto);
            _dbContext.SaveChanges();
        }

        public TradeAccount GetById(long id)
        {
            return _dbContext.TradeAccounts.FirstOrDefault(acc => !acc.Deleted && acc.Id == id);
        }

        public TradeAccount ChangeTradeRank(TradeAccount account)
        {
            var dto = GetById(account.Id);
            if (dto == null) return null;
            dto.TradeRank = account.TradeRank;
            _dbContext.SaveChanges();
            return dto;
        }

        public IQueryable<TradeNav> QueryTradeNav()
        {
            return _dbContext.TradeNavs;
        }

        public IQueryable<TradeSytossOpenPosition> QueryOpenPositions()
        {
            return _dbContext.TradeSytossOpenPositions;
        }
    }
}