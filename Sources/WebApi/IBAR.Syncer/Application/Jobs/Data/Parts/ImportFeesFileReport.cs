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
        private async Task<FileStatus> ProcessFeesFileReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

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
                            AccountName = _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId
                        });

                        var externalDate =
                            DateHelper.ParseDateTime(e.Attribute(XName.Get("date"))?.Value, "yyyyMMdd;HHmmss", DateHelper.DefaultDateFormat);

                        if (externalDate == null) throw new Exception("There is no date");

                        var newTradeFee = new TradeFee
                        {
                            TradeAccountId = tradeAccountId,
                            TradeFeeTypeId = tradeFeeTypeId,
                            TradeInstrumentId = tradeFeeInstrumentId,
                            // MasterAccountId = file.MasterAccountId,
                            // ----
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
                        Console.WriteLine($"Error while parsing TradeFee record. \nRow: {row} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                }
            }

            {
                var counter = 0;
                foreach (var tradeFee in newTradeFeeList)
                {
                    try
                    {
                        _importJobRepository.AddTradeFee(tradeFee);

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine($"Handled {counter} of {newTradeFeeList.Count} trade cash records");
                            logger.Log(LogLevel.Info,
                                $"import$ Handled {counter} of {newTradeFeeList.Count} trade cash records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while adding TradeFee record to DB. Exception: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }

                    counter++;
                }
            }

            _importJobRepository.SaveChanges();

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