using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IBAR.Syncer.Application.Helpers;
using IBAR.TradeModel.Data.Entities;
using NLog;

namespace IBAR.Syncer.Application.Jobs.Data
{
    internal partial class ImportJob
    {
        private async Task<FileStatus> ProcessNavFileReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

            var newTradeNavList = new List<TradeNav>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<EquitySummaryByReportDateInBase ")) continue;

                    try
                    {
                        var doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));

                        if (!doc.Elements().Any()) continue;

                        var e = doc.Elements().First();

                        var reportDate = DateHelper.ParseDate(e.Attribute(XName.Get("reportDate"))?.Value);
                        if (reportDate == null) continue;

                        var tradeAccId = GetOrCreateTradeAccountId(new TradeAccount
                        {
                            AccountName = _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId
                        });

                        var newTradeNav = new TradeNav
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = reportDate.Value,
                            Total = ParseDecimal(e.Attribute(XName.Get("total"))?.Value)
                        };

                        newTradeNavList.Add(newTradeNav);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing TradeNav record. \nRow: {row} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                }
            }

            {
                var counter = 0;
                foreach (var tradeNav in newTradeNavList)
                {
                    try
                    {
                        _importJobRepository.AddTradeNav(tradeNav);

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine($"Handled {counter} of {newTradeNavList.Count} trade nav records");
                            logger.Log(LogLevel.Info,
                                $"import$ Handled {counter} of {newTradeNavList.Count} trade nav records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while adding TradeNav record to DB. Exception: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }

                    counter++;
                }
            }
            
            _importJobRepository.SaveChanges();

            return await Task.FromResult(FileStatus.Success);
        }
    }
}