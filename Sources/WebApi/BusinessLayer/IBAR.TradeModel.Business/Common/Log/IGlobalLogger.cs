using NLog;
using System;

namespace IBAR.TradeModel.Business.Common.Log
{
    public interface IGlobalLogger
    {
        void LogInfo(string message, string jobName = null, bool displayToConsole = false);
        void LogError(string message, Exception ex = null, string jobName = null, bool displayToConsole = false);
        void LogBenchmark(string jobName, int threadId, string jobStatus, bool displayToConsole = false);
    }

    public interface IApiLogger
    {
        void LogError(string message, Guid guid, Exception ex = null);

        void LogInfo(string message);
    }

    public class BaseLogger
    {
        protected void DisplayToConsole(string message, bool displayToConsole)
        {
            if (displayToConsole)
            {
                Console.WriteLine(message);
            }
        }
    }

    public class IFileLogger : BaseLogger, IGlobalLogger
    {
        private readonly Logger _fileLog = LogManager.GetLogger("Logger");
        private readonly Logger _fileLogException = LogManager.GetLogger("LoggerException");

        public void LogInfo(string message, string jobName = null, bool displayToConsole = false)
        {
            var resultMsg = $"JOB: {jobName} | {message}";
            _fileLog.Info(resultMsg);
            DisplayToConsole(resultMsg, displayToConsole);
        }

        public void LogError(string message, Exception ex = null, string jobName = null, bool displayToConsole = false)
        {
            var resultMsg = $"JOB: {jobName} | {message}";
            _fileLogException.Error(ex, resultMsg);
            DisplayToConsole(resultMsg, displayToConsole);
        }

        public void LogBenchmark(string jobName, int threadId, string jobStatus, bool displayToConsole = false)
        {
            var resultMsg = $"JOB: {jobName} | STATUS: {jobStatus} | ThreadID: {threadId} - {DateTime.UtcNow}";
            _fileLog.Warn(resultMsg);
            DisplayToConsole(resultMsg, displayToConsole);
        }
    }

    public class IDbLogger : BaseLogger, IGlobalLogger
    {
        private const string JobColumn = "Job";
        private readonly static Logger _dbLog = LogManager.GetLogger("DBLogger");

        public void LogInfo(string message, string jobName = null, bool displayToConsole = false)
        {
            _dbLog.WithProperty(JobColumn, jobName).Info(message);
            DisplayToConsole(message, displayToConsole);
        }

        public void LogError(string message, Exception ex = null, string jobName = null, bool displayToConsole = false)
        {
            _dbLog.WithProperty(JobColumn, jobName).Error(ex, message);
            DisplayToConsole(message, displayToConsole);
        }

        public void LogBenchmark(string jobName, int threadId, string jobStatus, bool displayToConsole = false)
        {
            var resultMsg = $"JOB: {jobName} | STATUS: {jobStatus} | ThreadID: {threadId} - {DateTime.UtcNow}";
            _dbLog.WithProperty(JobColumn, jobName).Warn(resultMsg);
            DisplayToConsole(resultMsg, displayToConsole);
        }
    }

    public class IDbApiLogger : IApiLogger
    {
        private const string GuidColumn = "Guid";
        private readonly static Logger _logInfo = LogManager.GetLogger("DBLogInfo");
        private readonly static Logger _logError = LogManager.GetLogger("DBLogError");

        public void LogError(string message, Guid guid, Exception ex = null)
        {
            _logError.WithProperty(GuidColumn, guid).Error(ex, message);
        }

        public void LogInfo(string message)
        {
            _logInfo.Info(message);
        }
    }
}
