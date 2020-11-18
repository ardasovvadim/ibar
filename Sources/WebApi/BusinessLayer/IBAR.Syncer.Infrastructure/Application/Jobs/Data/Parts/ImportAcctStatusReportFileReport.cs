using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Application.Model;
using IBAR.TradeModel.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IBAR.Syncer.Infrastructure.Application.Jobs.Data
{
    public partial class ImportJob
    {
        private async Task<FileStatus> ProcessAcctStatusReportFileReport(ImportedFile file, string nameBaseNode,
            string sqlLikeExpression, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var tradeAccountsForAdding = new List<TradeAccount>();
            var tradeAccountsForUpdating = new List<TradeAccount>();

            if (stream.Position > 0) stream.Position = 0;

            var document = XDocument.Load(stream);
            var elements = document.Element(nameBaseNode)?.Element("Accounts")?.Elements("Account").ToList();

            if (elements != null && elements.Any())
            {
                elements.ForEach(e =>
                {
                    try
                    {
                        var tradeAccount = new TradeAccount
                        {
                            AccountName = _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("id"))?.Value),
                            MasterAccountId = file.MasterAccountId,
                            AccountAlias = e.Attribute(XName.Get("alias"))?.Value,
                            DateOpened = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_opened"))?.Value),
                            DateClosed = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_closed"))?.Value),
                            DateFunded = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_funded"))?.Value),
                            TradeStatus = e.Attribute(XName.Get("status"))?.Value,
                            Mobile = e.Attribute(XName.Get("mobile"))?.Value
                        };

                        if (_tradeAccountModel.Contains(tradeAccount.AccountName))
                        {
                            tradeAccount.Id = _tradeAccountModel.GetById(tradeAccount.AccountName).Id;
                            tradeAccount.ImportedFileId = file.Id;
                            tradeAccountsForUpdating.Add(tradeAccount);
                        }
                        else
                        {
                            tradeAccount.ImportedFile = file;
                            tradeAccountsForAdding.Add(tradeAccount);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeCash record. \nRow: {e}", ex, GetType().Name,
                            true);
                    }
                });
            }

            try
            {
                _importJobRepository.AddRangeTradeAccounts(tradeAccountsForAdding);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeAccount records to DB.", ex, GetType().Name,
                    true);
                return await Task.FromResult(FileStatus.Failed);
            }


            foreach (var tradeAccount in tradeAccountsForUpdating)
            {
                try
                {
                    _importJobRepository.AddOrUpdateTradeAccount(tradeAccount);
                }
                catch (Exception ex)
                {
                    GlobalLogger.LogError($"Error while updating TradeAccount record to DB.", ex, GetType().Name, true);
                    return await Task.FromResult(FileStatus.Failed);
                }
            }
            
            _importJobRepository.SaveChanges();

            GlobalLogger.LogInfo($"Trade Accounts: [{tradeAccountsForAdding.Count}] added.", GetType().Name, true);
            GlobalLogger.LogInfo($"Trade Accounts: [{tradeAccountsForUpdating.Count}] updated.", GetType().Name, true);


            RefillTradeAccounts();

            GC.Collect();

            return await Task.FromResult(FileStatus.Success);
        }

        private void RefillTradeAccounts()
        {
            var accounts = _importJobRepository.TradeAccountsQuery().Select(acc => new {acc.AccountName, acc.Id})
                .ToList();
            accounts.ForEach(acc =>
            {
                if (!_tradeAccountModel.Contains(acc.AccountName))
                {
                    _tradeAccountModel.Add(acc.AccountName, new TradeAccountModel {Id = acc.Id});
                }
            });
        }
    }
}