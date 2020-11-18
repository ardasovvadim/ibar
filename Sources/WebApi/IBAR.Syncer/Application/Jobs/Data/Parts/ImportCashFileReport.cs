using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Application.Model;
using IBAR.TradeModel.Data.Entities;
using NLog;

namespace IBAR.Syncer.Application.Jobs.Data
{
    internal partial class ImportJob
    {
        private async Task<FileStatus> ProcessCashReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

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

                        if (reportDate == null) throw new Exception(@"There is no fromDate");

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
                            MasterAccountId = file.MasterAccountId
                        });

                        var newTradeCash = new TradeCash
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = reportDate.Value,
                            Deposits = decimal.TryParse(e.Attribute(XName.Get("deposits"))?.Value,
                                out var deposits)
                                ? deposits
                                : 0,
                            Withdrawals = decimal.TryParse(e.Attribute(XName.Get("withdrawals"))?.Value,
                                out var withdrawals)
                                ? withdrawals
                                : 0
                        };

                        newTradeCashList.Add(newTradeCash);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing TradeCash record. \nRow: {row} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                }
            }

            {
                var counter = 0;
                foreach (var tradeCash in newTradeCashList)
                {
                    try
                    {
                        _importJobRepository.AddTradeCash(tradeCash);

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine($"Handled {counter} of {newTradeCashList.Count} trade cash records");
                            logger.Log(LogLevel.Info, $"import$ Handled {counter} of {newTradeCashList.Count} trade cash records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while adding TradeCash record to DB. Exception: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }

                    counter++;
                }
            }

            _importJobRepository.SaveChanges();

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