using IBAR.St.Toolkit.MemoryCache;
using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Application.Model;
using IBAR.Syncer.Infrastructure.Data;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.Jobs.Data
{
    public partial class ImportJob : BaseJob
    {
        private readonly IImportJobRepository _importJobRepository;

        private readonly InMemoryCache<TradeAccountModel> _tradeAccountModel;
        private readonly InMemoryCache<TradeInstrumentModel> _tradeInstrumentModel;
        private readonly InMemoryCache<TradeFeeTypeModel> _tradeFeeTypesModel;
        private readonly IExtractFileService _extractFileService;
        private readonly FileNameMatcher _fileNameMatcher;

        public ImportJob(
            IImportJobRepository importJobRepository,
            InMemoryCache<TradeAccountModel> tradeAcccountModel,
            InMemoryCache<TradeInstrumentModel> tradeInstrumentModel,
            InMemoryCache<TradeFeeTypeModel> tradeFeeTypesModel,
            FileNameMatcher fileNameMatcher,
            IExtractFileService extractFileService)
        {
            var jobInterval = ConfigurationManager.AppSettings["job:ImportJobInterval"];
            if (string.IsNullOrEmpty(jobInterval))
                throw new ConfigurationErrorsException("Please add 'job:ImportJobInterval' settigns to .config file.");

            JobInterval = int.Parse(jobInterval);

            _tradeAccountModel = tradeAcccountModel;
            _tradeInstrumentModel = tradeInstrumentModel;
            _tradeFeeTypesModel = tradeFeeTypesModel;
            _importJobRepository = importJobRepository;
            _extractFileService = extractFileService;
            _fileNameMatcher = fileNameMatcher;

            using (var tradeContext = _importJobRepository.BeginOperation())
            {
                foreach (var instrument in _importJobRepository.GetTradeInstrument())
                {
                    _tradeInstrumentModel.Add(instrument.InstrumentName,
                        new TradeInstrumentModel { Id = instrument.Id });
                }

                foreach (var trade in _importJobRepository.GetTradeAccount())
                {
                    _tradeAccountModel.Add(trade.AccountName, new TradeAccountModel { Id = trade.Id });
                }

                foreach (var tradeFeeType in _importJobRepository.GetTradeFeeType())
                {
                    _tradeFeeTypesModel.Add(tradeFeeType.TradeFeeTypeName,
                        new TradeFeeTypeModel { Id = tradeFeeType.Id });
                }
            }
        }

        protected async override Task RunInternal()
        {
            SyncerInfoService.Log(GetType().Name, JobStatus.Started.ToString());

            using (_importJobRepository.BeginOperation())
            {
                await ProcessImportFileQuery(_importJobRepository.LoadedFileQuery());

                await ProcessImportFileQuery(_importJobRepository.ImportedFailedFileQuery());
            }

            SyncerInfoService.Log(GetType().Name, JobStatus.Stopped.ToString());
        }

        private async Task ProcessImportFileQuery(IQueryable<ImportedFile> query)
        {
            // order is important for file processing
            var importFiles = query.OrderByDescending(f => f.OriginalFileName.StartsWith(_fileNameMatcher.AcctStatusReportNonEcaSqlLike))
                                    .ThenByDescending(f => f.OriginalFileName.StartsWith(_fileNameMatcher.AcctStatusReportSqlLike))
                                    .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.SytossClientInfoSqlLike))
                                    .ThenByDescending(f => f.FileCreateDate)
                                    .ToList();

            if (!importFiles.Any())
            {
                GlobalLogger.LogInfo("Job has not found new files for importing. Waiting...", GetType().Name, true);
            }
            else
            {
                foreach (var file in importFiles)
                {
                    try
                    {
                        await TryImportFile(file);
                    }
                    catch (Exception ex)
                    {
                        file.FileState = FileState.Imported;
                        file.FileStatus = FileStatus.Failed;
                        _importJobRepository.UpdateImportedFile(file);
                        _importJobRepository.SaveChanges();
                        GlobalLogger.LogError($"Error while importing file {file?.OriginalFileName}.", ex, GetType().Name, true);
                    }
                }
            }
        }

        private async Task TryImportFile(ImportedFile currentFile)
        {
            var newFileStatus = FileStatus.Success;
            try
            {
                if (currentFile != null)
                {
                    GlobalLogger.LogInfo($"Start importing file: {currentFile.OriginalFileName} | Status: {currentFile.FileStatus}", GetType().Name, true);

                    var streamFile = _extractFileService.ExtractFile(currentFile.OriginalFileName);

                    if (_fileNameMatcher.IsFeesDataFile(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessFeesFileReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsAcctStatusReportNonEca(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessAcctStatusReportFileReport(
                            currentFile,
                            "AccountStatusReport",
                            _fileNameMatcher.AcctStatusReportNonEcaSqlLike,
                            streamFile);
                    }
                    else if (_fileNameMatcher.IsNavRegex(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessNavFileReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsAcctStatusReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessAcctStatusReportFileReport(
                            currentFile,
                            "ECAAccountStatusReport",
                            _fileNameMatcher.AcctStatusReportSqlLike,
                            streamFile);
                    }
                    else if (_fileNameMatcher.IsCashReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessCashReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsTradeAsReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessTradesAsReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsSytossClientInfo(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessSytossClientInfoReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsSytossOpenPositions(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessSytossOpenPositions(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsTradeExeReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessTradesExeReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsInterestAccruaReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessTradeInterestAccruaReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsTradeCommissionsDetReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessTradeCommissionsReport(currentFile, streamFile);
                    }
                }

                GlobalLogger.LogInfo($"File: {currentFile.OriginalFileName} has been imported.", GetType().Name, true);
            }
            catch (Exception ex)
            {
                newFileStatus = FileStatus.Failed;
                GlobalLogger.LogError($"Error while importing file {currentFile?.OriginalFileName}.", ex, GetType().Name, true);
            }
            finally
            {
                if (currentFile != null)
                {
                    currentFile.FileState = FileState.Imported;
                    currentFile.FileStatus = newFileStatus;
                    if (newFileStatus == FileStatus.Success) currentFile.ImportedDate = DateTime.UtcNow;
                    
                    _importJobRepository.UpdateImportedFile(currentFile);
                    _importJobRepository.SaveChanges();
                }
                else
                {
                    GlobalLogger.LogInfo("Job has not found new files for importing. Waiting...", GetType().Name, true);
                }
            }
        }
    }
}