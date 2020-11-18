using System;
using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Business.Common.Extensions;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels.Request.Admin;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Admin;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface IAnalyticsService
    {
        AnalyticsCardInfoVm GetCardInfo(long masterAccountId, long ftpCredId);
        ChartDataVm GetChartData(AnalyticsChartDataParams @params);
    }

    public class AnalyticsService : IAnalyticsService
    {
        public AnalyticsService(IImportRepository importRepository, ISyncerInfoService syncerInfoService)
        {
            ImportRepository = importRepository;
            SyncerInfoService = syncerInfoService;
        }

        private IImportRepository ImportRepository { get; }
        private ISyncerInfoService SyncerInfoService { get; }

        public AnalyticsCardInfoVm GetCardInfo(long masterAccountId, long ftpCredId)
        {
            var result = new AnalyticsCardInfoVm();
            var dateNow = DateTime.UtcNow.AddDays(-1);
            
            var importedQuery = ImportRepository.ImportedFilesQuery();
            var zohoQuery = ImportRepository.FileUploadQuery().Include(f => f.ImportedFile);

            if (masterAccountId != 0)
            {
                importedQuery = importedQuery.Where(f => f.MasterAccountId == masterAccountId);
                zohoQuery = zohoQuery.Where(f => f.ImportedFile.MasterAccountId == masterAccountId);
            }

            if (ftpCredId != 0)
            {
                importedQuery = importedQuery.Where(f => f.FtpCredentialId == ftpCredId);
                zohoQuery = zohoQuery.Where(f => f.ImportedFile.FtpCredentialId == ftpCredId);
            }

            result.RegisteredFiles = importedQuery.Count(f => f.RegisteredDate != null &&
                                                              DbFunctions.TruncateTime(f.FileCreateDate) ==
                                                              DbFunctions.TruncateTime(dateNow));
            result.ImportedFiles = importedQuery.Count(f => f.ImportedDate != null &&
                                                            DbFunctions.TruncateTime(f.FileCreateDate) ==
                                                            DbFunctions.TruncateTime(dateNow));

            result.ZohoFiles = zohoQuery.Count(f => DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) ==
                                                    DbFunctions.TruncateTime(dateNow));
            
            result.SentFiles = zohoQuery.Count(f => f.IsSent &&
                                                    DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) ==
                                                    DbFunctions.TruncateTime(dateNow));

            result.LastSeenFtpJob = SyncerInfoService.GetLastSeen("FtpJob");
            result.LastSeenCopyJob = SyncerInfoService.GetLastSeen("CopyFromFtpJob");
            result.LastSeenImportJob = SyncerInfoService.GetLastSeen("ImportJob");
            result.LastSeenZohoJob = SyncerInfoService.GetLastSeen("FileDeliveryJob");

            return result;
        }

        public ChartDataVm GetChartData(AnalyticsChartDataParams @params)
        {
            var result = new ChartDataVm();

            var impQuery = ImportRepository.ImportedFilesQuery();
            var delQuery = ImportRepository.FileUploadQuery().Include(f => f.ImportedFile);

            if (@params.MasterAccountId != 0)
            {
                impQuery = impQuery.Where(f => f.MasterAccountId == @params.MasterAccountId);
                delQuery = delQuery.Where(f => f.ImportedFile.MasterAccountId == @params.MasterAccountId);
            }

            if (@params.FtpCredentialId != 0)
            {
                impQuery = impQuery.Where(f => f.FtpCredentialId == @params.FtpCredentialId);
                delQuery = delQuery.Where(f => f.ImportedFile.FtpCredentialId == @params.FtpCredentialId);
            }

            var srcFilesData = new ChartDataVm.DataSet {Label = "Source files"};
            var impFilesData = new ChartDataVm.DataSet {Label = "Imported files"};
            var delFilesData = new ChartDataVm.DataSet {Label = "ZOHO files"};
            var sentFilesData = new ChartDataVm.DataSet {Label = "Sent ZOHO files"};

            var periods = @params.Periods.ToPeriodList().ToList();

            periods.ToList().ForEach(period =>
            {
                var srcFiles = impQuery.Count(f =>
                    DbFunctions.TruncateTime(f.FileCreateDate) >= DbFunctions.TruncateTime(period.FromDate) &&
                    DbFunctions.TruncateTime(f.FileCreateDate) <= DbFunctions.TruncateTime(period.ToDate));

                var impFiles = impQuery.Count(f =>
                    DbFunctions.TruncateTime(f.FileCreateDate) >= DbFunctions.TruncateTime(period.FromDate) &&
                    DbFunctions.TruncateTime(f.FileCreateDate) <= DbFunctions.TruncateTime(period.ToDate) &&
                    f.ImportedDate != null);

                var delFiles = delQuery.Count(f =>
                    DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) >=
                    DbFunctions.TruncateTime(period.FromDate) &&
                    DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) <= DbFunctions.TruncateTime(period.ToDate));

                var sentFiles = delQuery.Count(f => f.IsSent &&
                                                    DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) >=
                                                    DbFunctions.TruncateTime(period.FromDate) &&
                                                    DbFunctions.TruncateTime(f.ImportedFile.FileCreateDate) <=
                                                    DbFunctions.TruncateTime(period.ToDate));

                srcFilesData.Data.Add(srcFiles);
                impFilesData.Data.Add(impFiles);
                delFilesData.Data.Add(delFiles);
                sentFilesData.Data.Add(sentFiles);
            });

            result.Data = new[] {srcFilesData, impFilesData, delFilesData, sentFilesData};
            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();

            return result;
        }
    }
}