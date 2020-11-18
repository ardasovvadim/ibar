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
        private async Task<FileStatus> ProcessTradesAsReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

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
                            MasterAccountId = file.MasterAccountId,
                            ImportedFile = file
                        });

                        var newTradeAs = new TradeTradesAs
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = file.FileCreateDate,
                            Quantity = ParseDecimal(e.Attribute(XName.Get("quantity"))?.Value),
                            AssetCategory = e.Attribute(XName.Get("assetCategory"))?.Value,
                            ImportedFile = file
                        };

                        newTradeAsList.Add(newTradeAs);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeAs record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeAs(newTradeAsList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeAs records to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }
    }
}