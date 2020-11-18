using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.Syncer.Infrastructure.Application.Model;
using IBAR.TradeModel.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IBAR.Syncer.Infrastructure.Application.Jobs.Data
{
    public partial class ImportJob
    {
        private async Task<FileStatus> ProcessCashReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var newTradeCashList = new List<TradeCash>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<CashReportCurrency ")) continue;

                    try
                    {
                        var document = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));
                        var el = document.Elements().First();
                        var reportDate = DateHelper.ParseDate(el.Attribute(XName.Get("fromDate"))?.Value);

                        if (reportDate == null)
                            throw new Exception(@"There is no fromDate");

                        await reader.ReadLineAsync();

                        if (!row.Trim().StartsWith("<CashReportCurrency ")) continue;

                        var doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));

                        if (!doc.Elements().Any()) continue;

                        var e = doc.Elements().First();

                        if (!(e.Attribute(XName.Get("currency")).Value == "BASE_SUMMARY")) continue;

                        var tradeAccId = GetOrCreateTradeAccountId(new TradeAccount
                        {
                            AccountName =
                                _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId,
                            ImportedFile = file
                        });

                        var newTradeCash = new TradeCash
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = reportDate.Value,
                            Deposits = ParseDecimal(e.Attribute(XName.Get("deposits"))?.Value),
                            Withdrawals = ParseDecimal(e.Attribute(XName.Get("withdrawals"))?.Value),
                            ImportedFile = file
                        };

                        newTradeCashList.Add(newTradeCash);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeCash record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeCash(newTradeCashList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeCash records to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }

        private long GetOrCreateTradeAccountId(TradeAccount tradeAcc)
        {
            if (_tradeAccountModel.Contains(tradeAcc.AccountName))
            {
                return _tradeAccountModel.GetById(tradeAcc.AccountName).Id;
            }

            var result = _importJobRepository.AddOrUpdateTradeAccount(tradeAcc);
            // for generated id
            _importJobRepository.SaveChanges();

            _tradeAccountModel.Add(result.AccountName, new TradeAccountModel {Id = result.Id});

            return result.Id;
        }
    }
}