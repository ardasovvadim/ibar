using IBAR.TradeModel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ITradeExeRepository
    {
        IQueryable<TradesExe> Query();
    }

    public class TradExeRepository : ITradeExeRepository
    {

        private TradeModelContext _dbContext;

        public TradExeRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TradesExe> Query()
        {
            return _dbContext.Set<TradesExe>();
        }
    }
}
