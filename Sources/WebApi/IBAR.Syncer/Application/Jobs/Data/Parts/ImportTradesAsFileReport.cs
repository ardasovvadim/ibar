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
        private async Task<FileStatus> ProcessTradesAsReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

            var newTradeAsList = new List<TradeTradesAs>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<AssetSummary ")) continue;

                    try
                    {
                        var doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));

                        if (!doc.Elements().Any()) continue;

                        var e = doc.Elements().First();

                        var tradeAccId = GetOrCreateTradeAccountId(new TradeAccount
                        {
                            AccountName =
                                _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId
                        });

                        var newTradeAs = new TradeTradesAs
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = file.FileCreateDate,
                            Quantity = ParseDecimal(e.Attribute(XName.Get("quantity"))?.Value),
                            AssetCategory = e.Attribute(XName.Get("assetCategory"))?.Value
                        };

                        newTradeAsList.Add(newTradeAs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing TradeAs record. \nRow: {row} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                }
            }

            {
                var counter = 0;
                foreach (var tradeAs in newTradeAsList)
                {
                    try
                    {
                        _importJobRepository.AddTradeAs(tradeAs);

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine($"Handled {counter} of {newTradeAsList.Count} trade as records");
                            logger.Log(LogLevel.Info,
                                $"import$ Handled {counter} of {newTradeAsList.Count} trade as records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while adding TradeAs record to DB. Exception: {ex}");
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