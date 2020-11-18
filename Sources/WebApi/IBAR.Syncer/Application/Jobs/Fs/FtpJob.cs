using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IBAR.St.Toolkit.MemoryCache;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Application.Model;
using IBAR.Syncer.Data;
using IBAR.Syncer.Tools.FileSystem.Ftp;
using IBAR.Syncer.Wcf;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using NLog;

namespace IBAR.Syncer.Application.Jobs.Fs
{
    internal class FtpJob : BaseJob
    {
        private readonly HashSet<string> _existingFiles = new HashSet<string>();

        private readonly FtpLoader _ftpLoader;
        private readonly IFtpJobRepository _ftpJobRepository;
        private readonly IImportRepository _importRepository;
        private readonly FileNameMatcher _fileNameMatcher;
        private readonly InMemoryCache<TradeMasterAccountModel> _tradeMasterAccountModel;
        private readonly DateTime _fromDate = DateTime.ParseExact(ConfigurationManager.AppSettings["startDate"],
            "yyyy-MM-dd", CultureInfo.InvariantCulture);

        public FtpJob(
            FtpLoader ftpLoader,
            InMemoryCache<TradeMasterAccountModel> tradeMasterAccountModel,
            IContract wcf,
            FileNameMatcher fileNameMatcher,
            IFtpJobRepository ftpJobRepository,
            IImportRepository importRepository) : base(wcf)
        {
            _job = Job.Ftp;
            _ftpLoader = ftpLoader;
            _tradeMasterAccountModel = tradeMasterAccountModel;
            _ftpJobRepository = ftpJobRepository;
            _importRepository = importRepository;
            _fileNameMatcher = fileNameMatcher;
            //using (_ftpJobRepository.BeginOperation())
            //{
            //    foreach (var ftpFile in ftpJobRepository.ImportedFilesQuery())
            //    {
            //        _existingFiles.Add(ftpFile.OriginalFileName);
            //    }
            //}
        }

        //    protected override async Task RunInternal()
        //    {
        //        using (_ftpJobRepository.BeginOperation())
        //        {
        //            var ftpCredentials = _ftpJobRepository.GetFtpCredentialsQuery().Where(cred => !cred.Deleted).ToList();

        //            if (!ftpCredentials.Any())
        //            {
        //                Console.WriteLine("FtpJob has not found any ftp credentials. Waiting...");
        //                logger.Log(LogLevel.Info, "FtpJob has not found any ftp credentials. Waiting...");
        //            }

        //            ftpCredentials.ForEach(ftpCred =>
        //            {
        //                var ftpName = ftpCred.FtpName;
        //                List<(string fileName, string accountName, DateTime dateCreation)> newFiles;
        //                // var masterAccountsName = _ftpJobRepository.MasterAccountsQuery().Select(acc => acc.AccountName).ToList();
        //                var masterAccountsName = ftpCred.MasterAccounts.Select(acc => acc.AccountName).ToList();

        //                try
        //                {
        //                    newFiles = _ftpLoader
        //                        .LoadFiles(ftpCred)?
        //                        .Where(file => !_existingFiles.Contains(file.fileName) &&
        //                                       masterAccountsName.Contains(file.accountName) &&
        //                                       file.dateCreation >= _fromDate)
        //                        .OrderByDescending(f => _fileNameMatcher.IsAcctStatusReportNonEca(f.fileName))
        //                        .ThenByDescending(f => _fileNameMatcher.IsAcctStatusReport(f.fileName))
        //                        .ThenBy(f => f.dateCreation)
        //                        .ToList();
        //                }
        //                catch (Exception e)
        //                {
        //                    logger.Log(LogLevel.Error, $"FtpJob: Error while trying to load files from ftp {ftpName}");
        //                    loggerException.Log(LogLevel.Error, e);
        //                    Console.WriteLine($"FtpJob: Error while trying to load files from ftp {ftpName}");
        //                    return;
        //                }

        //                FilterFilesByMaxFileCreateDateAcctStatusReport(newFiles,
        //                    _fileNameMatcher.AcctStatusReportNonEcaSqlLike,
        //                    ftpCred.MasterAccounts);
        //                FilterFilesByMaxFileCreateDateAcctStatusReport(newFiles,
        //                    _fileNameMatcher.AcctStatusReportSqlLike,
        //                    ftpCred.MasterAccounts);
        //                FilterFilesByMaxFileCreateDateAcctStatusReport(newFiles, 
        //                    _fileNameMatcher.SytossClientInfoSqlLike,
        //                    ftpCred.MasterAccounts);

        //                if (newFiles.Any())
        //                {
        //                    logger.Log(LogLevel.Info, $"ftp$Ftp job has found {newFiles.Count()} new files on {ftpName}");
        //                    Console.WriteLine($"Ftp job has found {newFiles.Count()} new files on {ftpName}");

        //                    var cnt = 0;

        //                    var transitFiles = _importRepository.GetTransitFileList();

        //                    foreach (var file in newFiles)
        //                    {
        //                        if (!_ftpJobRepository.ImportedFilesQuery().All(f => f.OriginalFileName != file.fileName))
        //                            continue;

        //                        var newFile = _ftpJobRepository.AddImportedFile(new ImportedFile()
        //                        {
        //                            CreatedDate = DateTime.UtcNow,
        //                            ModifiedDate = DateTime.UtcNow,
        //                            OriginalFileName = file.fileName,
        //                            FileCreateDate = file.dateCreation,
        //                            MasterAccount = _ftpJobRepository.GetMasterAccountByAccountName(file.accountName),
        //                            FtpCredential = ftpCred,
        //                            FileState = FileState.Registered,
        //                            FileStatus = FileStatus.Success,
        //                            IsForApi = transitFiles.Any(f => f.AccountName == file.accountName && file.fileName.Contains(f.OriginalFileName))
        //                        });

        //                        _existingFiles.Add(newFile.OriginalFileName);

        //                        cnt++;

        //                        logger.Log(LogLevel.Info,
        //                            $"ftp$Ftp file: {newFile.OriginalFileName} has been registered in database from {ftpName}");
        //                        Console.WriteLine(
        //                            $"Ftp file: {newFile.OriginalFileName} has been registered in database from {ftpName}");
        //                    }

        //                    _ftpJobRepository.SaveChanges();

        //                    if (cnt > 0)
        //                    {
        //                        logger.Log(LogLevel.Info,
        //                            $"ftp$Ftp file: {cnt} files have been registered in database from {ftpName}");
        //                        Console.WriteLine($"Ftp file: {cnt} files have been registered in database from {ftpName}");
        //                    }

        //                    logger.Log(LogLevel.Info, "ftp$FtpJob completed");
        //                    Console.WriteLine("FtpJob completed");
        //                }

        //                logger.Log(LogLevel.Info, $"ftp$FtpJob has not found new files in {ftpName}. Waiting...");
        //                Console.WriteLine($"FtpJob has not found new files in {ftpName}. Waiting...");
        //            });
        //        }
        //    }

        //    private void FilterFilesByMaxFileCreateDateAcctStatusReport(
        //        List<(string fileName, string accountName, DateTime dateCreation)> filesForDownloading,
        //        string sqlLikeExpression, IEnumerable<MasterAccount> masterAccounts)
        //    {
        //        // Get all new files acct_stat_report
        //        var allFiles =
        //            filesForDownloading.Where(file => file.fileName.Contains(sqlLikeExpression)).ToList();

        //        masterAccounts.ToList().ForEach(masterAccount =>
        //        {
        //            // Get last acct_stat_report (accumulated data)
        //            var currentFile = _ftpJobRepository.ImportedFilesQuery()
        //                .Where(curFile => curFile.OriginalFileName.Contains(sqlLikeExpression) &&
        //                                  curFile.MasterAccountId == masterAccount.Id)
        //                .OrderByDescending(currFile => currFile.FileCreateDate)
        //                .FirstOrDefault();
        //            var files = allFiles.Where(f => f.accountName == masterAccount.AccountName).ToList();
        //            if (!files.Any()) return;
        //            // Get with max update date
        //            var lastNewFile = files.FirstOrDefault(file =>
        //                // file.dateCreation == files.Max(f => f.dateCreation)
        //                file.dateCreation == files.Max(f => f.dateCreation)
        //            );
        //            // If better then current in database -> downloading and uploading in db
        //            if (currentFile == null || lastNewFile.dateCreation > currentFile.FileCreateDate)
        //            {
        //                files.Remove(lastNewFile);
        //            }

        //            filesForDownloading.RemoveAll(f => files.Contains(f));
        //        });
        //    }

        //    protected override int SleepInterval()
        //    {
        //        return 1000 * 60 * 15;
        //    }
        protected override Task RunInternal()
        {
            throw new NotImplementedException();
        }
    }
}