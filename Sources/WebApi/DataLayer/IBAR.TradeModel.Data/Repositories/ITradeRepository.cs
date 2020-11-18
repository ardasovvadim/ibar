using System.Linq;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ITradeRepository
    {
        IQueryable<TradeFee> TradeFeesQuery();
        IQueryable<MasterAccount> MasterAccountsQuery();
        IQueryable<TradeAccount> TradeAccountsQuery();
        IQueryable<TradeNav> TradeNavsQuery();
        IQueryable<TradeCash> TradeCashesQuery();
        IQueryable<TradeTradesAs> TradeAsQuery();
        IQueryable<TradeCommissions> TradeCommissionsQuery();
        IQueryable<TradeInterestAccrua> TradeInterestAccrua();

    }

    public class TradeRepository : ITradeRepository
    {
        private readonly TradeModelContext _dbContext;

        public TradeRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TradeFee> TradeFeesQuery()
        {
            return _dbContext.TradeFees;
        }

        public IQueryable<MasterAccount> MasterAccountsQuery()
        {
            return _dbContext.MasterAccounts;
        }

        public IQueryable<TradeAccount> TradeAccountsQuery()
        {
            return _dbContext.TradeAccounts;
        }

        public IQueryable<TradeNav> TradeNavsQuery()
        {
            return _dbContext.TradeNavs;
        }

        public IQueryable<TradeCash> TradeCashesQuery()
        {
            return _dbContext.TradeCashes;
        }

        public IQueryable<TradeTradesAs> TradeAsQuery()
        {
            return _dbContext.TradeTradesAs;
        }

        public IQueryable<TradeCommissions> TradeCommissionsQuery()
        {
            return _dbContext.TradeCommissions;
        }

        public IQueryable<TradeInterestAccrua> TradeInterestAccrua()
        {
            return _dbContext.TradeInterestAccruas;
        }
    }
}