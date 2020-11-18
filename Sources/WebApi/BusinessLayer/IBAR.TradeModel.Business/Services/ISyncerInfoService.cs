using System;
using System.Linq;
using IBAR.TradeModel.Data.Entities.Trade;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface ISyncerInfoService
    {
        void Log(string jobName, string message);
        DateTime? GetLastSeen(string jobName);
    }

    public class SyncerInfoService : ISyncerInfoService
    {
        private ISyncerInfoRepository SyncerInfoRepository { get; }
        private readonly object locker = new object();
        
        public SyncerInfoService(ISyncerInfoRepository syncerInfoRepository)
        {
            SyncerInfoRepository = syncerInfoRepository;
        }

        public void Log(string jobName, string message)
        {
            lock (locker)
            {
                var info = new SyncerInfo
                {
                    Message = message,
                    JobName = jobName,
                    TypeInfo = "Auto event"
                };
                
                SyncerInfoRepository.Add(info);
            }
        }

        public DateTime? GetLastSeen(string jobName)
        {
            return SyncerInfoRepository.Query().Where(inf => inf.JobName == jobName 
                                                             && inf.TypeInfo == "Auto event" 
                                                             && inf.Message == "Started")
                .OrderByDescending(inf => inf.CreatedDate).FirstOrDefault()?.CreatedDate;
        }
    }
}