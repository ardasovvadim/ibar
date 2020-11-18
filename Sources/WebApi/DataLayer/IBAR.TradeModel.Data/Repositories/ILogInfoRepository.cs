using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ILogInfoRepository
    {
        LogInfo Add(LogInfo logInfo);
        LogInfo GetLastByUserId(long id);
        void Save(LogInfo loginInfo);
    }

    public class LogInfoRepository : ILogInfoRepository
    {
        private readonly TradeModelContext _dbContext;

        public LogInfoRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LogInfo Add(LogInfo logInfo)
        {
            var log = _dbContext.LogInfos.Add(logInfo);
            _dbContext.SaveChanges();
            return log;
        }

        public LogInfo GetLastByUserId(long id)
        {
            var logs = _dbContext.LogInfos.Where(login => login.User.Id == id);
            return logs.FirstOrDefault(log => log.ExpiryDate == logs.Max(l => l.ExpiryDate));
        }

        public void Save(LogInfo loginInfo)
        {
            _dbContext.Entry(loginInfo).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}