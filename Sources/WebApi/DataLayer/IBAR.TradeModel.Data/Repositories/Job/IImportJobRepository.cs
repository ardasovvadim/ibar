using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IImportJobRepository : IBaseJobRepository
    {
        IQueryable<ImportedFile> LoadedFileQuery();
        IQueryable<ImportedFile> ImportedFailedFileQuery();
        IQueryable<TradeAccount> TradeAccountsQuery();
        TradeAccount AddOrUpdateTradeAccount(TradeAccount tradeAccount);
        void AddTradeNav(TradeNav nav);
        void AddTradeCash(TradeCash cash);
        void AddTradeFee(TradeFee tradeFee);
        TradeAccount GetTradeAccountById(long id);
        void AddTradeAs(TradeTradesAs tradeTradesAs);
        void AddTradeExe(TradesExe tradesExe);
        void UpdateTradeAccount(TradeAccount tradeAccount);
        IEnumerable<TradeAccount> GetTradeAccount();
        IEnumerable<TradeInstrument> GetTradeInstrument();
        IEnumerable<TradeFeeType> GetTradeFeeType();
        TradeFeeType AddTradeFeeType(TradeFeeType feeType);
        TradeInstrument AddTradeInstrument(TradeInstrument tradeInstrument);
        IEnumerable<MasterAccount> GetAllMasterAccounts();
        IQueryable<ImportedFile> ImportedFilesQuery();
        ImportedFile UpdateImportedFile(ImportedFile currentFile);
        TradingPermission GetTradePermissionByName(string name);
        TradingPermission AddTradePermission(string name);
        TradeAccount GetTradeAccountByAccountName(string tradeAccountAccountName);
        void AddRangeTradeAccounts(List<TradeAccount> tradeAccountsForAdding);
        void AddRangeTradeCash(List<TradeCash> newTradeCashList);
        void AddRangeTradeFee(List<TradeFee> newTradeFeeList);
        void AddRangeTradeNav(List<TradeNav> newTradeNavList);
        void AddRangeTradeAs(List<TradeTradesAs> newTradeAsList);
        void AddRangeTradeExe(List<TradesExe> newTradeInterestAccruaList);
        void AddRangeTradeInterestAccrua(List<TradeInterestAccrua> newTradeExeList);
        void AddRangeTradeCommissions(List<TradeCommissions> newTradeCommissions);
        void AddSytossOpenPosition(TradeSytossOpenPosition record);
        void AddRangeOpenPositions(List<TradeSytossOpenPosition> tradeAccSytossOpenPositionsRecords);
    }

    public class ImportJobRepository : BaseRepository, IImportJobRepository
    {
        public IQueryable<ImportedFile> LoadedFileQuery()
        {
            return _dbContext
                    .ImportedFiles
                    .Include(f => f.MasterAccount)
                    .Where(f => !f.Deleted && f.FileState == FileState.Loaded && f.FileStatus == FileStatus.Success);
        }

        public IQueryable<ImportedFile> ImportedFailedFileQuery()
        {
            return _dbContext
                    .ImportedFiles
                    .Include(f => f.MasterAccount)
                    .Where(f => !f.Deleted && f.FileState == FileState.Imported && f.FileStatus == FileStatus.Failed);
        }

        public IQueryable<TradeAccount> TradeAccountsQuery()
        {
            return _dbContext.Set<TradeAccount>();
        }

        public TradeAccount AddOrUpdateTradeAccount(TradeAccount tradeAccount)
        {
            _dbContext.TradeAccounts.AddOrUpdate(tradeAccount);
            return tradeAccount;
        }

        public void UpdateTradeAccount(TradeAccount tradeAccount)
        {
            _dbContext.TradeAccounts.AddOrUpdate(tradeAccount);
        }

        public void AddTradeNav(TradeNav nav)
        {
            _dbContext.TradeNavs.Add(nav);
        }

        public void AddTradeCash(TradeCash cash)
        {
            _dbContext.TradeCashes.Add(cash);
        }

        public TradeFeeType AddTradeFeeType(TradeFeeType feeType)
        {
            feeType = _dbContext.TradeFeeTypes.Add(feeType);
            return feeType;
        }

        public TradeInstrument AddTradeInstrument(TradeInstrument tradeInstrument)
        {
            tradeInstrument = _dbContext.TradeInstruments.Add(tradeInstrument);
            return tradeInstrument;
        }

        public IEnumerable<MasterAccount> GetAllMasterAccounts()
        {
            return _dbContext.MasterAccounts.Where(acc => !acc.Deleted);
        }

        public IQueryable<ImportedFile> ImportedFilesQuery()
        {
            return _dbContext.Set<ImportedFile>();
        }

        public ImportedFile UpdateImportedFile(ImportedFile currentFile)
        {
            _dbContext.Entry(currentFile).State = EntityState.Modified;
            return currentFile;
        }

        public TradingPermission GetTradePermissionByName(string name)
        {
            return _dbContext.TradingPermissions.FirstOrDefault(p => p.Name == name);
        }

        public TradingPermission AddTradePermission(string name)
        {
            return _dbContext.TradingPermissions.Add(new TradingPermission { Name = name });
        }

        public TradeAccount GetTradeAccountByAccountName(string tradeAccountAccountName)
        {
            return _dbContext.TradeAccounts.FirstOrDefault(acc =>
                !acc.Deleted && acc.AccountName == tradeAccountAccountName);
        }

        public void AddRangeTradeAccounts(List<TradeAccount> tradeAccountsForAdding)
        {
            _dbContext.TradeAccounts.AddRange(tradeAccountsForAdding);
        }

        public void AddRangeTradeCash(List<TradeCash> newTradeCashList)
        {
            _dbContext.TradeCashes.AddRange(newTradeCashList);
        }

        public void AddRangeTradeFee(List<TradeFee> newTradeFeeList)
        {
            _dbContext.TradeFees.AddRange(newTradeFeeList);
        }

        public void AddRangeTradeNav(List<TradeNav> newTradeNavList)
        {
            _dbContext.TradeNavs.AddRange(newTradeNavList);
        }

        public void AddRangeTradeAs(List<TradeTradesAs> newTradeAsList)
        {
            _dbContext.TradeTradesAs.AddRange(newTradeAsList);
        }

        public void AddRangeTradeExe(List<TradesExe> newTradeExeList)
        {
            _dbContext.TradesExes.AddRange(newTradeExeList);
        }

        public void AddSytossOpenPosition(TradeSytossOpenPosition record)
        {
            _dbContext.TradeSytossOpenPositions.Add(record);
        }

        public void AddRangeOpenPositions(List<TradeSytossOpenPosition> tradeAccSytossOpenPositionsRecords)
        {
            _dbContext.TradeSytossOpenPositions.AddRange(tradeAccSytossOpenPositionsRecords);
        }

        public void AddTradeAs(TradeTradesAs tradeTradesAs)
        {
            _dbContext.TradeTradesAs.Add(tradeTradesAs);
        }

        public void AddTradeFee(TradeFee tradeFee)
        {
            _dbContext.TradeFees.Add(tradeFee);
        }

        public void AddTradeExe(TradesExe tradesExe)
        {
            _dbContext.TradesExes.Add(tradesExe);
        }

        public TradeAccount GetTradeAccountById(long id)
        {
            return _dbContext
                    .TradeAccounts
                    .Include(x => x.TradingPermissions)
                    .FirstOrDefault(acc => acc.Id == id && !acc.Deleted);
        }

        public IEnumerable<TradeAccount> GetTradeAccount()
        {
            return _dbContext.TradeAccounts.Where(acc => !acc.Deleted);
        }

        public IEnumerable<TradeInstrument> GetTradeInstrument()
        {
            return _dbContext.TradeInstruments.Where(acc => !acc.Deleted);
        }

        public IEnumerable<TradeFeeType> GetTradeFeeType()
        {
            return _dbContext.TradeFeeTypes.Where(acc => !acc.Deleted);
        }

        public void AddRangeTradeInterestAccrua(List<TradeInterestAccrua> newTradeInterestAccruaList)
        {
            _dbContext.TradeInterestAccruas.AddRange(newTradeInterestAccruaList);
        }

        public void AddRangeTradeCommissions(List<TradeCommissions> newTradeCommissions)
        {
            _dbContext.TradeCommissions.AddRange(newTradeCommissions);
        }
    }
}