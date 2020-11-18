using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBAR.St.Toolkit.MemoryCache;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Application.Model;
using IBAR.Syncer.Data;
using IBAR.Syncer.Wcf;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using NLog;

namespace IBAR.Syncer.Application.Jobs.Data
{
    internal partial class ImportJob : BaseJob
    {
        private int _interval = 300;

        private readonly IImportJobRepository _importJobRepository;

        private readonly InMemoryCache<TradeAccountModel> _tradeAccountModel;
        private readonly InMemoryCache<TradeInstrumentModel> _tradeInstrumentModel;
        private readonly InMemoryCache<TradeFeeTypeModel> _tradeFeeTypesModel;
        private readonly InMemoryCache<TradeMasterAccountModel> _tradeMasterAccountModel;
        private readonly IExtractFileService _extractFileService;
        private readonly FileNameMatcher _fileNameMatcher;

        public ImportJob(
            IImportJobRepository importJobRepository,
            InMemoryCache<TradeAccountModel> tradeAcccountModel,
            InMemoryCache<TradeMasterAccountModel> tradeMasterAccountModel,
            InMemoryCache<TradeInstrumentModel> tradeInstrumentModel,
            InMemoryCache<TradeFeeTypeModel> tradeFeeTypesModel,
            IContract wcf,
            FileNameMatcher fileNameMatcher,
            IExtractFileService extractFileService) : base(wcf)
        {
            _job = Job.Import;

            _tradeAccountModel = tradeAcccountModel;
            _tradeInstrumentModel = tradeInstrumentModel;
            _tradeFeeTypesModel = tradeFeeTypesModel;
            _tradeMasterAccountModel = tradeMasterAccountModel;
            _importJobRepository = importJobRepository;
            _extractFileService = extractFileService;
            _fileNameMatcher = fileNameMatcher;
            using (var tradeContext = _importJobRepository.BeginOperation())
            {
                foreach (var masterAccount in _importJobRepository.GetAllMasterAccounts())
                {
                    _tradeMasterAccountModel.Add(masterAccount.AccountName,
                        new TradeMasterAccountModel { Id = masterAccount.Id });
                }

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
            using (_importJobRepository.BeginOperation())
            {
                var newFileStatus = FileStatus.Success;
                ImportedFile currentFile = null;
                try
                {
                    currentFile = _importJobRepository
                        .ImportedFilesQuery()
                        .OrderByDescending(
                            f => f.OriginalFileName.StartsWith(_fileNameMatcher.AcctStatusReportNonEcaSqlLike))
                        .ThenByDescending(f => f.OriginalFileName.StartsWith(_fileNameMatcher.AcctStatusReportSqlLike))
                        .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.SytossClientInfoSqlLike))
                        .ThenBy(f => f.FileCreateDate)
                        .FirstOrDefault(f => f.FileState == FileState.Loaded &&
                                             f.FileStatus == FileStatus.Success);

                    if (currentFile == null)
                    {
                        _interval *= 2;

                        if (_interval > 30000)
                        {
                            _interval = 30000;
                        }

                        return;
                    }

                    _interval = 300;

                    //retrieving steam of current file
                    var streamFile = _extractFileService.ExtractFile(currentFile.OriginalFileName);

                    if (_fileNameMatcher.IsFeesDataFile(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessFeesFileReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsAcctStatusReportNonEca(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessAcctStatusReportFileReport(currentFile, 
                            "AccountStatusReport",
                            _fileNameMatcher.AcctStatusReportNonEcaSqlLike, streamFile);
                    }
                    else if (_fileNameMatcher.IsNavRegex(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessNavFileReport(currentFile, streamFile);
                    }
                    else if (_fileNameMatcher.IsAcctStatusReport(currentFile.OriginalFileName))
                    {
                        newFileStatus = await ProcessAcctStatusReportFileReport(currentFile,
                            "ECAAccountStatusReport",
                            _fileNameMatcher.AcctStatusReportSqlLike, streamFile);
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
                }
                catch (Exception ex)
                {
                    newFileStatus = FileStatus.Failed;
                    loggerException.Error(this.GetErrorLogMessage(ex));
                }
                finally
                {
                    if (currentFile != null)
                    {
                        currentFile.FileState = FileState.Imported;
                        currentFile.FileStatus = newFileStatus;

                        _importJobRepository.UpdateImportedFile(currentFile);

                        _importJobRepository.SaveChanges();

                        if (newFileStatus == FileStatus.Failed)
                        {
                            Console.WriteLine($"ImportJob: Error while importing file {currentFile.OriginalFileName}");
                            logger.Log(LogLevel.Info,
                                $"import$ImportJob: Error while importing file {currentFile.OriginalFileName}");

                            Thread.Sleep(1000 * 10);
                            // ReloadThread();
                        }
                        else
                        {
                            Console.WriteLine($"ImportJob: {currentFile.OriginalFileName} has been imported");
                            logger.Log(LogLevel.Info,
                                $"import$ImportJob: {currentFile.OriginalFileName} has been imported");
                        }
                    }
                    else
                    {
                        logger.Log(LogLevel.Info,
                            $"import$ImportJob: has not found new files for downloading. Waiting...");
                        Console.WriteLine("ImportJob: has not found new files for downloading. Waiting...");
                    }
                }
            }
        }

        protected override int SleepInterval()
        {
            return _interval;
        }
    }
}