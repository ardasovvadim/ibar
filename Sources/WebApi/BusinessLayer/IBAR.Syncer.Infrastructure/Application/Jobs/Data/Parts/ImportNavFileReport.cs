using IBAR.Syncer.Infrastructure.Application.Helpers;
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
        private async Task<FileStatus> ProcessNavFileReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

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
                            AccountName =
                                _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId,
                            ImportedFile = file
                        });


                        var newTradeNav = new TradeNav
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = reportDate.Value,
                            Total = ParseDecimal(e.Attribute(XName.Get("total"))?.Value),
                            Cash = ParseDecimal(e.Attribute(XName.Get("cash"))?.Value),
                            Stock = ParseDecimal(e.Attribute(XName.Get("stock"))?.Value),
                            Options = ParseDecimal(e.Attribute(XName.Get("options"))?.Value),
                            Commodities = ParseDecimal(e.Attribute(XName.Get("commodities"))?.Value),
                            InterestAccruals = ParseDecimal(e.Attribute(XName.Get("interestAccruals"))?.Value),
                            
                            TotalLong = ParseDecimal(e.Attribute(XName.Get("totalLong"))?.Value),
                            CashLong = ParseDecimal(e.Attribute(XName.Get("cashLong"))?.Value),
                            StockLong = ParseDecimal(e.Attribute(XName.Get("stockLong"))?.Value),
                            OptionsLong = ParseDecimal(e.Attribute(XName.Get("optionsLong"))?.Value),
                            CommoditiesLong = ParseDecimal(e.Attribute(XName.Get("commoditiesLong"))?.Value),
                            InterestAccrualsLong = ParseDecimal(e.Attribute(XName.Get("interestAccrualsLong"))?.Value),
                            
                            TotalShort = ParseDecimal(e.Attribute(XName.Get("totalShort"))?.Value),
                            CashShort = ParseDecimal(e.Attribute(XName.Get("cashShort"))?.Value),
                            StockShort = ParseDecimal(e.Attribute(XName.Get("stockShort"))?.Value),
                            OptionsShort = ParseDecimal(e.Attribute(XName.Get("optionsShort"))?.Value),
                            CommoditiesShort = ParseDecimal(e.Attribute(XName.Get("commoditiesShort"))?.Value),
                            InterestAccrualsShort = ParseDecimal(e.Attribute(XName.Get("interestAccrualsShort"))?.Value),
                            
                            ImportedFile = file
                        };
                        newTradeNavList.Add(newTradeNav);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeNav record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeNav(newTradeNavList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeNav records to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }
    }
}