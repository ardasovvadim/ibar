using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.Syncer.Application.Helpers
{
    public class FileNameMatcher
    {
        private readonly IFileNameRegexRepository _fileNameRegexRepository;

        public FileNameMatcher(IFileNameRegexRepository fileNameRegexRepository)
        {
            _fileNameRegexRepository = fileNameRegexRepository;
            _supportedExtensions = ConfigurationManager.AppSettings["SupportedExtensions"]?.Split(',');
            if (_supportedExtensions == null)
            {
                throw new ConfigurationErrorsException("Please configure 'SupportedExtensions' settings in .config file.");
            }
        }

        public readonly string ClientFeesSqlLike = @"Sytoss-clientfees-det.";

        public readonly string AcctStatusReportNonEcaSqlLike = @"acct_status_report_non_eca.";

        public readonly string AcctStatusReportSqlLike = @"acct_status_report.";

        public readonly string SytossCashReportSqlLike = @"Sytoss-Cash.";

        public readonly string SytossNavSqlLike = @"Sytoss-Nav.";

        public readonly string SytossTradesAsSqlLike = @"Sytoss-Trades-ass.";

        public readonly string SytossClientInfoSqlLike = @"Sytoss-Client_info.";

        public static readonly string AccountId = @"[A-Za-z]{1}\d{5,7}";

        private readonly string[] _supportedExtensions;

        public bool IsMatch(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                   && _fileNameRegexRepository.GetFilePatterns().Any(s => Regex.IsMatch(fileName, s, RegexOptions.IgnoreCase)
                   && _supportedExtensions.Contains(Path.GetExtension(fileName)));
        }

        public string ParseAccountName(string fileName)
        {
            var result = Regex.Match(fileName, AccountId).Value;

            return result;
        }

        public bool IsFeesDataFile(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                   && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "ClientFeesRegex").FileRegex, RegexOptions.IgnoreCase);
        }

        public bool IsAcctStatusReportNonEca(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                   && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "AcctStatusReportNonEcaRegex").FileRegex, RegexOptions.IgnoreCase);
        }

        public bool IsNavRegex(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                   && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "SytossNavRegex").FileRegex, RegexOptions.IgnoreCase);
        }

        public bool IsAcctStatusReport(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                   && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "AcctStatusReportRegex").FileRegex, RegexOptions.IgnoreCase);
        }

        public bool IsCashReport(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "SytossCashReportRegex").FileRegex, RegexOptions.IgnoreCase);
        }
        public bool IsTradeAsReport(string fileName)
        {
            return !string.IsNullOrEmpty(fileName)
                && Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "SytossTradesAsRegex").FileRegex, RegexOptions.IgnoreCase);
        }

        public bool IsSytossClientInfo(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) &&
                   Regex.IsMatch(fileName, _fileNameRegexRepository.FileNameRegexesQuery().FirstOrDefault(f => f.FileName == "SytossClientInfoRegex").FileRegex, RegexOptions.IgnoreCase);
        }
        
        public string GetCorrectAccountId(string fileName)
        {
            var accId = Regex.Match(fileName, AccountId, RegexOptions.IgnoreCase);
            return accId.Value;
        }
    }
}