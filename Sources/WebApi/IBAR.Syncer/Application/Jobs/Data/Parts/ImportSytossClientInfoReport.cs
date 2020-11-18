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
        private async Task<FileStatus> ProcessSytossClientInfoReport(ImportedFile file, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

            var tradeAccountsForUpdating = new List<TradeAccount>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var row = await reader.ReadLineAsync();

                    if (!row.Trim().StartsWith("<AccountInformation")) continue;

                    try
                    {
                        var doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(row)));

                        if (!doc.Elements().Any()) continue;

                        var e = doc.Elements().First();

                        long tradeAccId;
                        tradeAccId = GetOrCreateTradeAccountId(new TradeAccount
                        {
                            AccountName =
                                _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("accountId"))?.Value),
                            AccountAlias = e.Attribute(XName.Get("acctAlias"))?.Value,
                            MasterAccountId = file.MasterAccountId
                        });

                        var tradeAccount = _importJobRepository.GetTradeAccountById(tradeAccId);

                        var tradingPermsStr = e.Attribute(XName.Get("tradingPermissions"))?.Value;
                        ProcessTradingPermissions(tradingPermsStr, tradeAccount);

                        tradeAccount.Name = e.Attribute(XName.Get("name"))?.Value;
                        tradeAccount.City = e.Attribute(XName.Get("city"))?.Value;
                        tradeAccount.DateFunded = DateHelper.ParseDate(e.Attribute(XName.Get("dateFunded"))?.Value);
                        tradeAccount.СountryResidentialAddress =
                            e.Attribute(XName.Get("countryResidentialAddress"))?.Value;
                        tradeAccount.CityResidentialAddress = e.Attribute(XName.Get("cityResidentialAddress"))?.Value;
                        tradeAccount.StateResidentialAddress = e.Attribute(XName.Get("stateResidentialAddress"))?.Value;
                        tradeAccount.IbEntity = e.Attribute(XName.Get("ibEntity"))?.Value;
                        tradeAccount.AccountType = e.Attribute(XName.Get("accountType"))?.Value;
                        tradeAccount.StreetResidentialAddress =
                            e.Attribute(XName.Get("streetResidentialAddress"))?.Value;
                        tradeAccount.PostalCode = e.Attribute(XName.Get("postalCode"))?.Value;
                        tradeAccount.PrimaryEmail = e.Attribute(XName.Get("primaryEmail"))?.Value;
                        tradeAccount.Currency = e.Attribute(XName.Get("currency"))?.Value;
                        tradeAccount.AccountCapabilities = e.Attribute(XName.Get("accountCapabilities"))?.Value;
                        tradeAccount.CustomerType = e.Attribute(XName.Get("customerType"))?.Value;
                        tradeAccount.IsClientInfo = true;

                        tradeAccountsForUpdating.Add(tradeAccount);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing ClientInfo record. \nRow: {row} \nException: {ex}");
                        loggerException.Log(LogLevel.Error,
                            $"Error while parsing ClientInfo record. \nRow: {row} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                }
            }

            {
                var counter = 0;
                foreach (var tradeAccount in tradeAccountsForUpdating)
                {
                    try
                    {
                        _importJobRepository.UpdateTradeAccount(tradeAccount);

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine(
                                $"Handled {counter} of {tradeAccountsForUpdating.Count} client info records");
                            logger.Log(LogLevel.Info,
                                $"import$ Handled {counter} of {tradeAccountsForUpdating.Count} client info records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while updating Client info of TradeAccount. Exception: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }

                    counter++;
                }
            }

            _importJobRepository.SaveChanges();

            RefillTradeAccounts();

            return await Task.FromResult(FileStatus.Success);
        }

        private void ProcessTradingPermissions(string tradingPermsStr, TradeAccount tradeAccount)
        {
            if (tradingPermsStr == null) return;

            var tradingPerms = tradingPermsStr.Split(',').ToList();
            var accountTradePerms = tradeAccount.TradingPermissions.Select(per => per.Name).ToList();
            var permsForAdding = tradingPerms.Except(accountTradePerms).ToList();
            var permsForDeleting = accountTradePerms.Except(tradingPerms).ToList();
            permsForAdding.ForEach(perm =>
            {
                var dto = GetOrCreateTradingPermission(perm);
                tradeAccount.TradingPermissions.Add(dto);
            });
            permsForDeleting.ForEach(perm =>
            {
                var dto = GetOrCreateTradingPermission(perm);
                tradeAccount.TradingPermissions.Remove(dto);
            });
        }

        private TradingPermission GetOrCreateTradingPermission(string name)
        {
            var permission = _importJobRepository.GetTradePermissionByName(name);

            if (permission == null)
            {
                permission = _importJobRepository.AddTradePermission(name);
                _importJobRepository.SaveChanges();
            }

            return permission;
        }
    }
}