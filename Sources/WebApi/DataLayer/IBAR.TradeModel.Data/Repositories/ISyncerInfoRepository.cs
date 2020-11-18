using System.Linq;
using IBAR.TradeModel.Data.Entities.Trade;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ISyncerInfoRepository
    {
        void Add(SyncerInfo info);
        IQueryable<SyncerInfo> Query();
    }

    public class SyncerInfoRepository : ISyncerInfoRepository
    {
        private TradeModelContext Context { get; }
        
        public SyncerInfoRepository()
        {
            Context = new TradeModelContext();
        }


        public void Add(SyncerInfo info)
        {
            Context.SyncerInfos.Add(info);
            Context.SaveChanges();
        }

        public IQueryable<SyncerInfo> Query()
        {
            return Context.SyncerInfos.Where(inf => !inf.Deleted);
        }
    }
}