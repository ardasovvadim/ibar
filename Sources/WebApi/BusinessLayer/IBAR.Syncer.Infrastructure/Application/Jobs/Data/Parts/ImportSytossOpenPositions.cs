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
        private async Task<FileStatus> ProcessSytossOpenPositions(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var tradeAccSytossOpenPositionsRecords = new List<TradeSytossOpenPosition>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<OpenPosition ")) continue;

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

                        var record = new TradeSytossOpenPosition
                        {
                            TradeAccountId = tradeAccId,

                            ReportDate = file.FileCreateDate,
                            Symbol = e.Attribute(XName.Get("symbol"))?.Value,
                            Position = ParseDecimal(e.Attribute(XName.Get("position"))?.Value),
                            MarkPrice = ParseDecimal(e.Attribute(XName.Get("markPrice"))?.Value),
                            CostBasisPrice = ParseDecimal(e.Attribute(XName.Get("costBasisPrice"))?.Value),
                            CostBasisMoney = ParseDecimal(e.Attribute(XName.Get("costBasisMoney"))?.Value),
                            PercentOfNav = ParseDecimal(e.Attribute(XName.Get("percentOfNAV"))?.Value),
                            FifoPnlUnrealized = ParseDecimal(e.Attribute(XName.Get("fifoPnlUnrealized"))?.Value),
                            Description = e.Attribute(XName.Get("description"))?.Value,
                            AssetCategory = e.Attribute(XName.Get("assetCategory"))?.Value,
                            UnderlyingListingExchange = e.Attribute(XName.Get("underlyingListingExchange"))?.Value,
                            Currency = e.Attribute(XName.Get("currency"))?.Value,
                            Isin = e.Attribute(XName.Get("isin"))?.Value,
                            Conid = ParseDecimal(e.Attribute(XName.Get("conid"))?.Value),
                            FxRateToBase = ParseDecimal(e.Attribute(XName.Get("fxRateToBase"))?.Value),
                            UnderlyingSymbol = e.Attribute(XName.Get("underlyingSymbol"))?.Value,
                            PutCall = e.Attribute(XName.Get("putCall"))?.Value,
                            Multiplier = ParseDecimal(e.Attribute(XName.Get("multiplier"))?.Value),
                            Strike = ParseDecimal(e.Attribute(XName.Get("strike"))?.Value),
                            Expiry = DateHelper.ParseDate(e.Attribute(XName.Get("expiry"))?.Value),
                            
                            ImportedFile = file
                        };

                        tradeAccSytossOpenPositionsRecords.Add(record);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing SytossOpenPositions record. \nRow: {row}", ex,
                            GetType().Name, true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeOpenPositions(tradeAccSytossOpenPositionsRecords);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding SytossOpenPosition.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            RefillTradeAccounts();

            return await Task.FromResult(FileStatus.Success);
        }
    }
}