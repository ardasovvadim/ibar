using IBAR.Syncer.Infrastructure.Application.FileSystemProviders;
using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Data;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.Jobs.Fs
{
    public class CopyFromFtpJob : BaseJob
    {
        private readonly ICopyJobRepository _copyJobRepository;
        private readonly IFileManagerService _systemManager;
        private readonly FileNameMatcher _fileNameMatcher;

        public CopyFromFtpJob(
            ICopyJobRepository copyJobRepository,
            IFileManagerService systemManager,
            FileNameMatcher fileNameMatcher)
        {
            var jobInterval = ConfigurationManager.AppSettings["job:CopyFromFtpJobInterval"];
            if (string.IsNullOrEmpty(jobInterval))
                throw new ConfigurationErrorsException("Please add 'job:CopyFromFtpJobInterval' settigns to .config file.");

            JobInterval = int.Parse(jobInterval);

            _copyJobRepository = copyJobRepository;
            _systemManager = systemManager;
            _fileNameMatcher = fileNameMatcher;
        }

        protected async override Task RunInternal()
        {
            SyncerInfoService.Log(GetType().Name, JobStatus.Started.ToString());

            using (_copyJobRepository.BeginOperation())
            {
                try
                {
                    var registeredFiles = _copyJobRepository
                            .RegisteredFilesQuery()
                            // order is important for file processing
                            .OrderByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.AcctStatusReportNonEcaSqlLike))
                            .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.AcctStatusReportSqlLike))
                            .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.SytossClientInfoSqlLike))
                            .ThenByDescending(f => f.FileCreateDate)
                            .ToList();

                    if (!registeredFiles.Any())
                    {
                        GlobalLogger.LogInfo("Job has not found new files for downloading. Waiting...", GetType().Name, true);
                    }
                    else
                    {
                        GlobalLogger.LogInfo($"Job has found new files: [{registeredFiles.Count()}] for downloading.", GetType().Name, true);

                        var totalCount = 0;
                        foreach (var file in registeredFiles)
                        {
                            totalCount++;

                            await TryLoadFileAsync(file);

                            GlobalLogger.LogInfo($"Amount copied: [{totalCount}] of [{registeredFiles.Count()}].", GetType().Name, true);
                        }
                    }

                    var loadedFailedFiles = _copyJobRepository.LoadedFailedFileList();

                    if (!loadedFailedFiles.Any())
                    {
                        GlobalLogger.LogInfo("Job has not found LOAD FAILED FILES. Waiting...", GetType().Name, true);
                    }
                    else
                    {
                        GlobalLogger.LogInfo($"Job has found LOAD FAILED FILES: [{loadedFailedFiles.Count()}].", GetType().Name, true);

                        var failedCount = 0;
                        foreach (var file in loadedFailedFiles)
                        {
                            failedCount++;

                            await TryLoadFileAsync(file);

                            GlobalLogger.LogInfo($"Amount of recopied (failed files): [{failedCount}] of [{loadedFailedFiles.Count()}].", GetType().Name, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobalLogger.LogError($"Error while copying files from ftp.", ex, GetType().Name, true);
                }

            }

            SyncerInfoService.Log(GetType().Name, JobStatus.Stopped.ToString());
        }

        private async Task TryLoadFileAsync(ImportedFile file)
        {
            try
            {
                file.FileState = FileState.Loaded;
                file.FileStatus = FileStatus.Success;
                file.CopiedDate = DateTime.UtcNow;
                await _systemManager.SaveFileAsync(file);
                GlobalLogger.LogInfo($"File copied: {file.OriginalFileName} from ftp: [{file.FtpCredential.FtpName}].", GetType().Name, true);
            }
            catch (Exception ex)
            {
                file.FileStatus = FileStatus.Failed;
                GlobalLogger.LogError($"Error while copying file: {file.OriginalFileName} from ftp: [{file.FtpCredential.FtpName}].", ex, GetType().Name, true);
            }
            finally
            {
                _copyJobRepository.UpdateImportedFile(file);
                _copyJobRepository.SaveChanges();
            }
        }
    }
}