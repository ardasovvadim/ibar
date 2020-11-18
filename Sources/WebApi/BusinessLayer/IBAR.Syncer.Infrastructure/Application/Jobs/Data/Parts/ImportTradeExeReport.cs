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
        private async Task<FileStatus> ProcessTradesExeReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var newTradeExeList = new List<TradesExe>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<Trade ")) continue;

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

                        var newTradeExe = new TradesExe
                        {
                            TradeAccountId = tradeAccId,
                            Quantity = ParseDecimal(e.Attribute(XName.Get("quantity"))?.Value),
                            TradePrice = ParseDecimal(e.Attribute(XName.Get("tradePrice"))?.Value),
                            ClosePrice = ParseDecimal(e.Attribute(XName.Get("closePrice"))?.Value),
                            VolatilityOrderLink =
                                long.TryParse(e.Attribute(XName.Get("volatilityOrderLink"))?.Value, out var res)
                                    ? res
                                    : 0,
                            AssetCategory = e.Attribute(XName.Get("assetCategory"))?.Value,
                            PutCall = e.Attribute(XName.Get("putCall"))?.Value,
                            BuySell = e.Attribute(XName.Get("buySell"))?.Value,
                            Description = e.Attribute(XName.Get("description"))?.Value,
                            OrderType = e.Attribute(XName.Get("orderType"))?.Value,
                            OrderReference = e.Attribute(XName.Get("orderReference"))?.Value,
                            Currency = e.Attribute(XName.Get("currency"))?.Value,
                            IbExecId = e.Attribute(XName.Get("ibExecID"))?.Value,
                            OpenCloseIndicator = e.Attribute(XName.Get("openCloseIndicator"))?.Value,
                            Isin = e.Attribute(XName.Get("isin"))?.Value,
                            Symbol = e.Attribute(XName.Get("symbol"))?.Value,
                            IsAPIOrder = e.Attribute(XName.Get("isAPIOrder"))?.Value,
                            Multiplier = ParseDecimal(e.Attribute(XName.Get("multiplier"))?.Value),
                            TransactionID =
                                long.TryParse(e.Attribute(XName.Get("transactionID"))?.Value, out var result)
                                    ? result
                                    : 0,
                            IbOrderID = long.TryParse(e.Attribute(XName.Get("ibOrderID"))?.Value, out var resul)
                                ? resul
                                : 0,
                            IbCommission = ParseDecimal(e.Attribute(XName.Get("ibCommission"))?.Value),
                            Conid = ParseDecimal(e.Attribute(XName.Get("conid"))?.Value),
                            Strike = ParseDecimal(e.Attribute(XName.Get("strike"))?.Value),
                            Taxes = ParseDecimal(e.Attribute(XName.Get("taxes"))?.Value),
                            FxRateToBase = ParseDecimal(e.Attribute(XName.Get("fxRateToBase"))?.Value),
                            Expiry = DateHelper.ParseDateTime(e.Attribute(XName.Get("expiry"))?.Value, "yyyyMMdd",
                                DateHelper.DefaultDateFormat),
                            OrderTime = DateHelper.ParseDateTime(e.Attribute(XName.Get("orderTime"))?.Value,
                                "yyyyMMdd;HHmmss", DateHelper.DefaultDateFormat),
                            SettleDateTarget = DateHelper.ParseDateTime(
                                e.Attribute(XName.Get("settleDateTarget"))?.Value, "yyyyMMdd",
                                DateHelper.DefaultDateFormat),
                            ReportDate = DateHelper.ParseDateTime(e.Attribute(XName.Get("reportDate"))?.Value,
                                "yyyyMMdd", DateHelper.DefaultDateFormat),
                            ListingExchange = e.Attribute(XName.Get("listingExchange"))?.Value,
                            UnderlyingSymbol = e.Attribute(XName.Get("underlyingSymbol"))?.Value,
                            ImportedFile = file
                        };

                        newTradeExeList.Add(newTradeExe);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeExe record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeExe(newTradeExeList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeAs TradeExe to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }
    }
}