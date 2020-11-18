using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;
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
        private async Task<FileStatus> ProcessTradeInterestAccruaReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return await Task.FromResult(FileStatus.Failed);

            var newTradeInterestAccruaList = new List<TradeInterestAccrua>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<InterestAccrualsCurrency ")) continue;

                    try
                    {
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

                        var newTradeInterestAccrua = new TradeInterestAccrua
                        {
                            TradeAccountId = tradeAccId,
                            ReportDate = file.FileCreateDate,
                            EndingAccrualBalance = ParseDecimal(e.Attribute(XName.Get("endingAccrualBalance"))?.Value),
                            ImportedFile = file
                        };

                        newTradeInterestAccruaList.Add(newTradeInterestAccrua);
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger.LogError($"Error while parsing InterestAccrua record. \nRow: {row}", ex, GetType().Name,
                            true);
                    }
                }
            }

            try
            {
                _importJobRepository.AddRangeTradeInterestAccrua(newTradeInterestAccruaList);
                _importJobRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                GlobalLogger.LogError($"Error while adding InterestAccrua records to DB.", ex, GetType().Name, true);
                return await Task.FromResult(FileStatus.Failed);
            }

            return await Task.FromResult(FileStatus.Success);
        }
    }
}
