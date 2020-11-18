using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Data;
using IBAR.Syncer.Infrastructure.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.Jobs.Fs
{
    public class FtpJob : BaseJob
    {
        private readonly IFtpJobRepository _ftpJobRepository;
        private readonly FtpLoader _ftpLoader;
        private readonly FileNameMatcher _fileNameMatcher;
        private readonly DateTime _fromDate;

        public FtpJob(
            FtpLoader ftpLoader,
            FileNameMatcher fileNameMatcher,
            IFtpJobRepository ftpJobRepository)
        {
            var fromDateSetting = ConfigurationManager.AppSettings["startDate"];
            if (string.IsNullOrEmpty(fromDateSetting))
                throw new ConfigurationErrorsException("Please add 'startDate' settigns to .config file.");

            var jobInterval = ConfigurationManager.AppSettings["job:FtpJobInterval"];
            if (string.IsNullOrEmpty(jobInterval))
                throw new ConfigurationErrorsException("Please add 'job:FtpJobInterval' settigns to .config file.");

            _fromDate = DateTime.ParseExact(fromDateSetting, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            JobInterval = int.Parse(jobInterval);

            _ftpJobRepository = ftpJobRepository;
            _ftpLoader = ftpLoader;
            _fileNameMatcher = fileNameMatcher;
        }

        protected override async Task RunInternal()
        {
            SyncerInfoService.Log(GetType().Name, JobStatus.Started.ToString());

            using (_ftpJobRepository.BeginOperation())
            {
                var ftpCredentials = _ftpJobRepository.GetFtpCredentialList();

                if (!ftpCredentials.Any())
                {
                    GlobalLogger.LogInfo("Job has not found any ftp credentials. Waiting...", GetType().Name, true);
                }
                else
                {
                    var existingFiles = _ftpJobRepository.ImportedFileList();
                    var existingFileNames = existingFiles.Select(ef => ef.OriginalFileName);

                    foreach (var cred in ftpCredentials)
                    {
                        var totalCount = 0;
                        List<(string fileName, string accountName, DateTime creationDate)> newFiles;
                        var accountNameList = cred.MasterAccounts.Select(m => m.AccountName).ToList();

                        try
                        {
                            newFiles = (await _ftpLoader.LoadFilesAsync(cred))
                                .Where(f => f.creationDate >= _fromDate && accountNameList.Contains(f.accountName) && !existingFileNames.Contains(f.fileName))
                                // order is important for file processing
                                .OrderByDescending(f => _fileNameMatcher.IsAcctStatusReportNonEca(f.fileName))
                                .ThenByDescending(f => _fileNameMatcher.IsAcctStatusReport(f.fileName))
                                .ThenByDescending(f => f.creationDate)
                                .ToList();
                        }
                        catch (Exception ex)
                        {
                            GlobalLogger.LogError($"Error while trying to load files from ftp: [{cred.FtpName}]", ex, GetType().Name, true);
                            return;
                        }

                        FilterFilesByMaxCreateDate(newFiles, existingFiles, _fileNameMatcher.AcctStatusReportNonEcaSqlLike, cred.MasterAccounts);
                        FilterFilesByMaxCreateDate(newFiles, existingFiles, _fileNameMatcher.AcctStatusReportSqlLike, cred.MasterAccounts);
                        FilterFilesByMaxCreateDate(newFiles, existingFiles, _fileNameMatcher.SytossClientInfoSqlLike, cred.MasterAccounts);

                        if (!newFiles.Any())
                        {
                            GlobalLogger.LogInfo($"Job has not found new files in [{cred.FtpName}]. Waiting...", GetType().Name, true);
                        }
                        else
                        {
                            GlobalLogger.LogInfo($"Job has found new: [{newFiles.Count()}] files on ftp: [{cred.FtpName}].", GetType().Name, true);

                            var transitFiles = _ftpJobRepository.GetTransitFileList();

                            foreach (var file in newFiles)
                            {
                                if (!existingFiles.All(f => f.OriginalFileName != file.fileName))
                                    continue;

                                var newFile = new ImportedFile()
                                {
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow,
                                    OriginalFileName = file.fileName,
                                    FileCreateDate = file.creationDate,
                                    MasterAccount = cred.MasterAccounts.FirstOrDefault(a => a.AccountName == file.accountName),
                                    FtpCredential = cred,
                                    FileState = FileState.Registered,
                                    FileStatus = FileStatus.Success,
                                    RegisteredDate = DateTime.UtcNow
                                };

                                if (transitFiles.Any(f => f.AccountName == file.accountName && file.fileName.Contains(f.OriginalFileName)))
                                {
                                    newFile.FileUpload = new FileUpload();
                                }

                                _ftpJobRepository.AddImportedFile(newFile);

                                totalCount++;

                                GlobalLogger.LogInfo($"New file: {newFile.OriginalFileName} registered from ftp: [{cred.FtpName}].", GetType().Name, true);
                            }


                            try
                            {
                                _ftpJobRepository.SaveChanges();
                                GlobalLogger.LogInfo($"Amount registered: [{totalCount}] files in DB from ftp: [{cred.FtpName}].", GetType().Name, true);
                            }
                            catch (Exception ex)
                            {
                                GlobalLogger.LogError($"Error while trying to save registered files from ftp: [{cred.FtpName}]", ex, GetType().Name, true);
                                return;
                            }
                        }
                    }
                }
            }

            SyncerInfoService.Log(GetType().Name, JobStatus.Stopped.ToString());
        }

        private void FilterFilesByMaxCreateDate(
            List<(string fileName, string accountName, DateTime dateCreation)> newFiles,
            IEnumerable<ImportedFile> existingFiles,
            string sqlLikeExpression,
            IEnumerable<MasterAccount> masterAccounts)
        {
            var matchedNewFiles = newFiles.Where(f => f.fileName.Contains(sqlLikeExpression));

            foreach (var account in masterAccounts)
            {
                var currentFile = existingFiles
                    .Where(f => f.OriginalFileName.Contains(sqlLikeExpression) && f.MasterAccountId == account.Id)
                    .OrderByDescending(f => f.FileCreateDate)
                    .FirstOrDefault();

                var files = matchedNewFiles.Where(f => f.accountName == account.AccountName).ToList();
                if (files.Any())
                {
                    var lastNewFile = files.OrderByDescending(f => f.dateCreation).FirstOrDefault();

                    if (currentFile == null || lastNewFile.dateCreation > currentFile.FileCreateDate)
                    {
                        files.Remove(lastNewFile);
                    }

                    newFiles.RemoveAll(f => files.Contains(f));
                }
            }
        }
    }
}