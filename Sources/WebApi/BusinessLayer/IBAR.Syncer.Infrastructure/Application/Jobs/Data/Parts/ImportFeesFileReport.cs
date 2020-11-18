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
        private async Task<FileStatus> ProcessFeesFileReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var newTradeFeeList = new List<TradeFee>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<ClientFee ")) continue;

                    try
                    {
                        var doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));
                        var e = doc.Elements().First();

                        var tradeFeeTypeId = GetOrCreateTradeFeeTypeId(new TradeFeeType
                        {
                            TradeFeeTypeName = e.Attribute(XName.Get("feeType"))?.Value
                        });

                        var tradeFeeInstrumentId = GetOrCreateTradeFeeInstrumentId(new TradeInstrument
                        {
                            InstrumentName = e.Attribute(XName.Get("currency"))?.Value
                        });

                        var tradeAccountId = GetOrCreateTradeAccountId(new TradeAccount
                        {
                            AccountName =
                                _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId,
                            ImportedFile = file
                        });

                        var externalDate = DateHelper.ParseDateTime(e.Attribute(XName.Get("date"))?.Value,
                            "yyyyMMdd;HHmmss", DateHelper.DefaultDateFormat);

                        if (externalDate == null)
                            throw new Exception("There is no date");

                        var newTradeFee = new TradeFee
                        {
                            TradeAccountId = tradeAccountId,
                            TradeFeeTypeId = tradeFeeTypeId,
                            TradeInstrumentId = tradeFeeInstrumentId,
                            ImportedFile = file,

                            ExternalTradeName = e.Attribute(XName.Get("tradeID"))?.Value,
                            ExternalExecName = e.Attribute(XName.Get("execID"))?.Value,
                            ExternalDate = externalDate.Value,
                            RateToBase = ParseDecimal(e.Attribute(XName.Get("fxRateToBase"))?.Value),
                            RevenueInCurrency = ParseDecimal(e.Attribute(XName.Get("revenue"))?.Value),
                            RevenueInBase = ParseDecimal(e.Attribute(XName.Get("revenueInBase"))?.Value),
                            ExpenseInCurrency = ParseDecimal(e.Attribute(XName.Get("expense"))?.Value),
                            ExpenseInBase = ParseDecimal(e.Attribute(XName.Get("expenseInBase"))?.Value),
                            NetInCurrency = ParseDecimal(e.Attribute(XName.Get("net"))?.Value),
                            NetInBase = ParseDecimal(e.Attribute(XName.Get("netInBase"))?.Value),
                        };

                        newTradeFeeList.Add(newTradeFee);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing TradeFee record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeFee(newTradeFeeList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding TradeFee records to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }

        private long GetOrCreateTradeFeeInstrumentId(TradeInstrument tradeInstrument)
        {
            if (_tradeInstrumentModel.Contains(tradeInstrument.InstrumentName))
            {
                return _tradeInstrumentModel.GetById(tradeInstrument.InstrumentName).Id;
            }

            var result = _importJobRepository.AddTradeInstrument(tradeInstrument);
            _importJobRepository.SaveChanges();

            _tradeInstrumentModel.Add(result.InstrumentName, new TradeInstrumentModel {Id = result.Id});

            return result.Id;
        }

        private long GetOrCreateTradeFeeTypeId(TradeFeeType tradeFeeType)
        {
            if (_tradeFeeTypesModel.Contains(tradeFeeType.TradeFeeTypeName))
            {
                return _tradeFeeTypesModel.GetById(tradeFeeType.TradeFeeTypeName).Id;
            }

            var result = _importJobRepository.AddTradeFeeType(tradeFeeType);
            _importJobRepository.SaveChanges();

            _tradeFeeTypesModel.Add(result.TradeFeeTypeName, new TradeFeeTypeModel {Id = result.Id});

            return result.Id;
        }

        private static decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0;
        }
    }
}