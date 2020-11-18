using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IBaseJobRepository
    {
        IDisposable BeginOperation();
        void SaveChanges();
        Task SaveChangesAsync();
    }
    public abstract class BaseRepository : IBaseJobRepository
    {
        protected TradeModelContext _dbContext;
        public void SaveChanges()
        {
            _dbContext.ChangeTracker.DetectChanges();

            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        public async Task SaveChangesAsync()
        {
            _dbContext.ChangeTracker.DetectChanges();

            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    await ex.Entries.Single().ReloadAsync();
                }

            } while (saveFailed);
        }

        public IDisposable BeginOperation()
        {
            _dbContext = new TradeModelContext();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            return _dbContext;
        }
    }
}