using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IBAR.Syncer.Application.FileSystemProviders;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Data;
using IBAR.Syncer.Wcf;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using NLog;

namespace IBAR.Syncer.Application.Jobs.Fs
{
    public class FsCopyFromFtpJob : BaseJob
    {
        private readonly string _historyDir = ConfigurationManager.AppSettings["historyFolder"];
        private readonly ICopyJobRepository _copyJobRepository;
        private readonly IFileSystemManager _systemManager;
        private readonly FileNameMatcher _fileNameMatcher;


        public FsCopyFromFtpJob(IFileSystemManager systemManager, IContract wcf, ICopyJobRepository copyJobRepository, FileNameMatcher fileNameMatcher) :
            base(wcf)
        {
            _job = Job.Copy;
            lock (_syncRoot)
            {
                if (!Directory.Exists(_historyDir))
                {
                    Directory.CreateDirectory(_historyDir);
                }
            }

            _systemManager = systemManager;
            _copyJobRepository = copyJobRepository;
            _fileNameMatcher = fileNameMatcher;
        }

        //    protected async override Task RunInternal()
        //    {
        //        using (_copyJobRepository.BeginOperation())
        //        {
        //            ImportedFile currentFile = null;
        //            var ftpName = "";
        //            // if file will be registered successfully
        //            var newFileStatus = FileStatus.Success;

        //            try
        //            {
        //                currentFile = _copyJobRepository
        //                    .ImportedFilesQuery()
        //                    .OrderByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.AcctStatusReportNonEcaSqlLike))
        //                    .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.AcctStatusReportSqlLike))
        //                    .ThenByDescending(f => f.OriginalFileName.Contains(_fileNameMatcher.SytossClientInfoSqlLike))
        //                    .ThenBy(f => f.FileCreateDate)
        //                    .FirstOrDefault(f => f.FileState == FileState.Registered &&
        //                                         f.FileStatus == FileStatus.Success);

        //                if (currentFile != null)
        //                {
        //                    ftpName = currentFile.FtpCredential.FtpName;
        //                    _systemManager.SaveFile(currentFile);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (currentFile != null)
        //                {
        //                    logger.Log(LogLevel.Error,
        //                        $"CopyJob: Error while copying file {currentFile.OriginalFileName} from ftp {ftpName}");
        //                    Console.WriteLine(
        //                        $"CopyJob: Error while copying file {currentFile.OriginalFileName} from ftp {ftpName}");
        //                    newFileStatus = FileStatus.Failed;
        //                }
        //                else
        //                {
        //                    logger.Log(LogLevel.Error, "CopyJob: Error while copying file from ftp");
        //                    Console.WriteLine("CopyJob: Error while copying file from ftp");
        //                }

        //                loggerException.Error(this.GetErrorLogMessage(ex));
        //            }
        //            finally
        //            {
        //                if (currentFile != null)
        //                {
        //                    currentFile.FileState = FileState.Loaded;
        //                    currentFile.FileStatus = newFileStatus;

        //                    _copyJobRepository.UpdateImportedFile(currentFile);

        //                    _copyJobRepository.SaveChanges();

        //                    Console.WriteLine(
        //                        $"CopyJob: {currentFile.OriginalFileName} has been copied from ftp {ftpName}");
        //                    logger.Log(LogLevel.Info,
        //                        $"copy$CopyJob: {currentFile.OriginalFileName}  was copied from ftp {ftpName}");
        //                }
        //                else
        //                {
        //                    Console.WriteLine("CopyJob has not found new files for downloading. Waiting...");
        //                    logger.Log(LogLevel.Info, "copy$CopyJob has not found new files for downloading. Waiting...");
        //                }
        //            }

        //            await Task.FromResult(true);
        //        }
        //    }

        //    protected override int SleepInterval()
        //    {
        //        return 1000 * 15;
        //    }
        protected override Task RunInternal()
        {
            throw new NotImplementedException();
        }
    }
}