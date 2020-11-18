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
        /*private async Task<FileStatus> ProcessAcctStatusReportFileReport(ImportedFile file, string nameBaseNode,
            string sqlLikeExpression, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);


            // Find last file
            var nameCurrentFile = _importJobRepository.ImportedFilesQuery()
                .Where(curFile => curFile.OriginalFileName.Contains(sqlLikeExpression)
                                  && curFile.FileState == FileState.Imported &&
                                  curFile.FileStatus == FileStatus.Success &&
                                  curFile.MasterAccountId == file.MasterAccountId)
                .OrderByDescending(currFile => currFile.FileCreateDate)
                .FirstOrDefault();

            // If exists the same earlier files, so it will be need compare
            if (nameCurrentFile != null)
            {
                var streamCurrentFile = _extractFileService.ExtractFile(nameCurrentFile);
                var streamNewFile = stream;

                var xDocCurrFile = XDocument.Load(streamCurrentFile);
                var xDocNewFile = XDocument.Load(streamNewFile);

                // If detect new changes, else we just parsing needed information
                if (!XmlHelper.CompareXml(xDocCurrFile, xDocNewFile, out var resultXml))
                    return await Task.FromResult(FileStatus.Success);

                var tradeAccountForUpdating = new List<TradeAccount>();
                var tradeAccountsForAdding = new List<TradeAccount>();

                // xDocCurrFile./

                // Old xml
                var currSource = xDocCurrFile;
                var currRoot = currSource.Element(nameBaseNode)?.Element("Accounts")?.Elements("Account")
                    .ToList();

                // Xml with detected changes
                var newDoc = XDocument.Load(new StringReader(resultXml));
                var newRoot = newDoc.Element("xmldiff")?.Element("node")?.Element("node");

                if (newRoot == null || currRoot == null)
                    return await Task.FromResult(FileStatus.Failed);

                // CHANGES
                var changedAccounts = newRoot.Elements("node").ToList();
                if (changedAccounts.Any())
                {
                    changedAccounts.ForEach(account =>
                    {
                        //  existing account (account name)
                        var match = account.Attribute("match")?.Value;
                        if (match == null) return;
                        var indexNode = int.Parse(match) - 1;
                        if (indexNode >= currRoot.Count) return;
                        var accountName = currRoot[indexNode].Attribute("id")?.Value;
                        accountName = _fileNameMatcher.GetCorrectAccountId(accountName);

                        var existingAccount =
                            _importJobRepository.GetTradeAccountById(_tradeAccountModel.GetById(accountName).Id);
                        var changed = false;

                        // tracked fields for updating, must be with prefix @
                        var trackedChangeAttributes = new[]
                            {"@status", "@date_opened", "@date_closed", "@alias", "@date_funded", "@mobile"};
                        var changes = account.Elements("change")
                            .Where(c => trackedChangeAttributes.Contains(c.Attribute("match")?.Value)).ToList();
                        // update fields
                        changes.ForEach(c =>
                        {
                            var value = c.Value;
                            switch (c.Attribute("match")?.Value)
                            {
                                case "@status":
                                {
                                    existingAccount.TradeStatus = value;
                                    changed = true;
                                    break;
                                }
                                case "@date_opened":
                                    existingAccount.DateOpened = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "@date_closed":
                                    existingAccount.DateClosed = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "@alias":
                                    existingAccount.AccountAlias = value;
                                    changed = true;
                                    break;
                                case "@date_funded":
                                    existingAccount.DateFunded = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "@mobile":
                                    existingAccount.Mobile = value;
                                    changed = true;
                                    break;
                            }
                        });

                        // tracked fields for adding in change account
                        var trackedAddAttributes = new[]
                            {"status", "date_opened", "date_closed", "alias", "date_funded", "mobile"};
                        var adds = account.Elements("add")
                            .Where(c => trackedAddAttributes.Contains(c.Attribute("name")?.Value)).ToList();
                        adds.ForEach(add =>
                        {
                            var value = add.Value;
                            switch (add.Attribute("name")?.Value)
                            {
                                case "status":
                                {
                                    existingAccount.TradeStatus = value;
                                    changed = true;
                                    break;
                                }
                                case "date_opened":
                                    existingAccount.DateOpened = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "date_closed":
                                    existingAccount.DateClosed = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "alias":
                                    existingAccount.AccountAlias = value;
                                    changed = true;
                                    break;
                                case "date_funded":
                                    existingAccount.DateFunded = DateHelper.ParseDateTime(value);
                                    changed = true;
                                    break;
                                case "mobile":
                                    existingAccount.Mobile = value;
                                    changed = true;
                                    break;
                            }
                        });

                        if (changed)
                        {
                            tradeAccountForUpdating.Add(existingAccount);
                        }
                    });
                }

                // NEW ACCOUNTS
                var addedNodes = newRoot.Elements("add").ToList();

                if (addedNodes.Any())
                {
                    // New accounts between exists
                    var addedAccountBetween = addedNodes.Where(node =>
                        node.Attributes().Any(attr => attr.Name == "name" && attr.Value == "Account")).ToList();

                    if (addedAccountBetween.Any())
                    {
                        addedAccountBetween.ForEach(account =>
                        {
                            var xElements = account.Elements("add").ToList();
                            var newTradeAccount = new TradeAccount
                            {
                                AccountName = XmlHelper.GetValueBetweenAdded(xElements, "id"),
                                MasterAccountId = file.MasterAccountId,
                                DateOpened =
                                    DateHelper.ParseDateTime(XmlHelper.GetValueBetweenAdded(xElements, "date_opened")),
                                DateClosed =
                                    DateHelper.ParseDateTime(XmlHelper.GetValueBetweenAdded(xElements, "date_closed")),
                                AccountAlias = XmlHelper.GetValueBetweenAdded(xElements, "alias"),
                                DateFunded =
                                    DateHelper.ParseDateTime(XmlHelper.GetValueBetweenAdded(xElements, "date_funded")),
                                TradeStatus = XmlHelper.GetValueBetweenAdded(xElements, "status"),
                                Mobile = XmlHelper.GetValueBetweenAdded(xElements, "mobile")
                            };

                            tradeAccountsForAdding.Add(newTradeAccount);
                        });
                    }

                    var addedAccountsBlocks = addedNodes.Where(node => !node.HasAttributes).ToList();

                    if (addedAccountsBlocks.Any())
                    {
                        var addedAccounts = new List<XElement>();
                        addedAccountsBlocks.Select(block => block.Elements("Account")).ToList().ForEach(list =>
                        {
                            var xElements = list.ToList();
                            if (xElements.Any())
                            {
                                addedAccounts.AddRange(xElements);
                            }
                        });
                        if (addedAccounts.Any())
                        {
                            addedAccounts.ForEach(account =>
                            {
                                var newTradeAccount = new TradeAccount
                                {
                                    AccountName = account.Attribute("id")?.Value,
                                    MasterAccountId = file.MasterAccountId,
                                    DateOpened = DateHelper.ParseDateTime(account.Attribute("date_opened")?.Value),
                                    DateClosed = DateHelper.ParseDateTime(account.Attribute("date_closed")?.Value),
                                    AccountAlias = account.Attribute("alias")?.Value,
                                    DateFunded = DateHelper.ParseDateTime(account.Attribute("date_funded")?.Value),
                                    TradeStatus = account.Attribute("status")?.Value,
                                    Mobile = account.Attribute("mobile")?.Value
                                };

                                tradeAccountsForAdding.Add(newTradeAccount);
                            });
                        }
                    }

                    {
                        var counter = 0;
                        tradeAccountForUpdating.ForEach(acc =>
                        {
                            try
                            {
                                if (_tradeAccountModel.Contains(acc.AccountName))
                                {
                                    _importJobRepository.UpdateTradeAccount(acc);
                                }
                            }
                            catch (Exception ex)
                            {
                                loggerException.Error(
                                    this.GetErrorLogMessage(
                                        $"Error while updating exists trade account {acc.AccountName}"));
                                loggerException.Error(this.GetErrorLogMessage(ex));
                                Console.WriteLine(
                                    $"ImportJob: Error while updating exists trade account {acc.AccountName}");
                                return;
                            }

                            if (counter % 100 == 0)
                            {
                                Console.WriteLine(
                                    $"Handled {counter} of {tradeAccountForUpdating.Count} trade accounts records and updated");
                                logger.Log(LogLevel.Info,
                                    $"import$ Handled {counter} of {tradeAccountForUpdating.Count} trade accounts records and updated");
                            }

                            ++counter;
                            // logger.Log(LogLevel.Info,
                            //     $"import$ImportJob: Trade account {acc.AccountName} has been updated");
                            // Console.WriteLine($"ImportJob: Trade account {acc.AccountName} has been updated");
                        });
                    }

                    {
                        var counter = 0;
                        tradeAccountsForAdding.ForEach(acc =>
                        {
                            try
                            {
                                if (!_tradeAccountModel.Contains(acc.AccountName))
                                {
                                    _importJobRepository.AddTradeAccount(acc);
                                }
                                else
                                {
                                    var yetExistingErrorAcc =
                                        _importJobRepository.GetTradeAccountById(_tradeAccountModel
                                            .GetById(acc.AccountName)
                                            .Id);
                                    yetExistingErrorAcc.TradeStatus = acc.TradeStatus;
                                    yetExistingErrorAcc.DateOpened = acc.DateOpened;
                                    yetExistingErrorAcc.DateClosed = acc.DateClosed;
                                    yetExistingErrorAcc.AccountAlias = acc.AccountAlias;
                                    yetExistingErrorAcc.MasterAccountId = acc.MasterAccountId;
                                    yetExistingErrorAcc.DateFunded = acc.DateFunded;
                                    yetExistingErrorAcc.Mobile = acc.Mobile;

                                    _importJobRepository.UpdateTradeAccount(yetExistingErrorAcc);

                                    // logger.Log(LogLevel.Info,
                                    //     $"import$ImportJob: Trade account {acc.AccountName} has been updated");
                                    // Console.WriteLine($"ImportJob: Trade account {acc.AccountName} has been updated");
                                }
                            }
                            catch (Exception ex)
                            {
                                loggerException.Error(
                                    this.GetErrorLogMessage($"Error while adding new trade account {acc.AccountName}"));
                                loggerException.Error(this.GetErrorLogMessage(ex));
                                Console.WriteLine($"ImportJob: Error while adding new trade account {acc.AccountName}");
                                return;
                            }

                            if (counter % 100 == 0)
                            {
                                Console.WriteLine(
                                    $"Handled {counter} of new {tradeAccountsForAdding.Count} trade accounts records and added");
                                logger.Log(LogLevel.Info,
                                    $"import$ Handled {counter} of new {tradeAccountsForAdding.Count} trade accounts records and added");
                            }

                            ++counter;

                            // logger.Log(LogLevel.Info, $"import$ImportJob: Trade account {acc.AccountName} has been added");
                            // Console.WriteLine($"ImportJob: Trade account {acc.AccountName} has been added");
                        });
                    }

                    _importJobRepository.Save();

                    RefillTradeAccounts();

                    return await Task.FromResult(FileStatus.Success);
                }
            }

            // If name current file is null it means that database is empty
            var pathNewFile = stream;
            var xDoc = XDocument.Load(pathNewFile);
            var xRoot = xDoc.Element(nameBaseNode)?.Element("Accounts");

            if (xRoot == null) return await Task.FromResult(FileStatus.Failed);

            var newAccounts = new List<TradeAccount>();
            var accounts = xRoot.Elements("Account").ToList();

            accounts.ForEach(account =>
            {
                var newTradeAccount = new TradeAccount
                {
                    AccountName = account.Attribute("id")?.Value,
                    MasterAccountId = file.MasterAccountId,
                    DateOpened = DateHelper.ParseDateTime(account.Attribute("date_opened")?.Value),
                    DateClosed = DateHelper.ParseDateTime(account.Attribute("date_closed")?.Value),
                    AccountAlias = account.Attribute("alias")?.Value,
                    DateFunded = DateHelper.ParseDateTime(account.Attribute("date_funded")?.Value),
                    TradeStatus = account.Attribute("status")?.Value,
                    Mobile = account.Attribute("mobile")?.Value
                };

                newAccounts.Add(newTradeAccount);
            });

            {
                var counter = 0;

                newAccounts.ForEach(acc =>
                {
                    try
                    {
                        if (!_tradeAccountModel.Contains(acc.AccountName))
                        {
                            _importJobRepository.AddTradeAccount(acc);
                        }
                        else
                        {
                            var yetExistingErrorAcc =
                                _importJobRepository.GetTradeAccountById(_tradeAccountModel.GetById(acc.AccountName)
                                    .Id);
                            yetExistingErrorAcc.TradeStatus = acc.TradeStatus;
                            yetExistingErrorAcc.DateOpened = acc.DateOpened;
                            yetExistingErrorAcc.DateClosed = acc.DateClosed;
                            yetExistingErrorAcc.AccountAlias = acc.AccountAlias;
                            yetExistingErrorAcc.MasterAccountId = acc.MasterAccountId;
                            yetExistingErrorAcc.DateFunded = acc.DateFunded;
                            yetExistingErrorAcc.Mobile = acc.Mobile;

                            _importJobRepository.UpdateTradeAccount(yetExistingErrorAcc);

                            logger.Log(LogLevel.Info,
                                $"import$ImportJob: Trade account {acc.AccountName} has been updated");
                            Console.WriteLine($"ImportJob: Trade account {acc.AccountName} has been updated");
                        }
                    }
                    catch (Exception ex)
                    {
                        loggerException.Error(
                            this.GetErrorLogMessage($"Error while adding new trade account {acc.AccountName}."));
                        loggerException.Error(this.GetErrorLogMessage(ex));
                        Console.WriteLine($"ImportJob: Error while adding new trade account {acc.AccountName}");
                        return;
                    }

                    if (counter % 100 == 0)
                    {
                        Console.WriteLine($"Handled {counter} of {newAccounts.Count} trade account records");
                        logger.Log(LogLevel.Info,
                            $"import$ Handled {counter} of {newAccounts.Count} trade account records");
                    }

                    ++counter;
                    // logger.Log(LogLevel.Info, $"import$ImportJob: Trade account {acc.AccountName} has been added");
                    // Console.WriteLine($"ImportJob: Trade account {acc.AccountName} has been added");
                });
            }

            _importJobRepository.Save();

            RefillTradeAccounts();

            logger.Log(LogLevel.Info,
                $"import$ImportJob: {accounts.Count()} accounts have been detected and added");
            Console.WriteLine($"ImportJob: {accounts.Count()} accounts have been detected and added");


            return await Task.FromResult(FileStatus.Success);
        }*/

        private async Task<FileStatus> ProcessAcctStatusReportFileReport(ImportedFile file, string nameBaseNode,
            string sqlLikeExpression, Stream stream)
        {
            if (stream == null || stream.Length == 0) return await Task.FromResult(FileStatus.Failed);

            var tradeAccounts = new List<TradeAccount>();

            if (stream.Position > 0) stream.Position = 0;

            var document = XDocument.Load(stream);
            var elements = document.Element(nameBaseNode)?.Element("Accounts")?.Elements("Account").ToList();

            if (elements != null && elements.Any())
            {
                elements.ForEach(e =>
                {
                    try
                    {
                        var newTradeAccount = new TradeAccount
                        {
                            AccountName = _fileNameMatcher.GetCorrectAccountId(e.Attribute(XName.Get("id"))?.Value),
                            MasterAccountId = file.MasterAccountId,
                            AccountAlias = e.Attribute(XName.Get("alias"))?.Value,
                            DateOpened = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_opened"))?.Value),
                            DateClosed = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_closed"))?.Value),
                            DateFunded = DateHelper.ParseDateTime(e.Attribute(XName.Get("date_funded"))?.Value),
                            TradeStatus = e.Attribute(XName.Get("status"))?.Value,
                            Mobile = e.Attribute(XName.Get("mobile"))?.Value
                        };

                        tradeAccounts.Add(newTradeAccount);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing TradeCash record. \nRow: {e} \nException: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }
                });
            }

            {
                var counter = 0;
                var newAccCounter = 0;
                var updatingAccCounter = 0;
                foreach (var tradeAccount in tradeAccounts)
                {
                    try
                    {
                        if (_tradeAccountModel.Contains(tradeAccount.AccountName))
                        {
                            var existingAccount = _importJobRepository.GetTradeAccountByAccountName(tradeAccount.AccountName);
                            existingAccount.AccountAlias = tradeAccount.AccountAlias;
                            existingAccount.DateOpened = tradeAccount.DateOpened;
                            existingAccount.DateClosed = tradeAccount.DateClosed;
                            existingAccount.DateFunded = tradeAccount.DateFunded;
                            existingAccount.TradeStatus = tradeAccount.TradeStatus;
                            existingAccount.Mobile = tradeAccount.Mobile;

                            ++updatingAccCounter;
                        }
                        else
                        {
                            _importJobRepository.AddOrUpdateTradeAccount(tradeAccount);

                            ++newAccCounter;
                        }

                        if (counter % 100 == 0)
                        {
                            Console.WriteLine(
                                $"ImportJob: Handled {counter} of {tradeAccounts.Count} trade accounts records");
                            logger.Log(LogLevel.Info,
                                $"import$ImportJob: Handled {counter} of {tradeAccounts.Count} trade accounts records");
                            _importJobRepository.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while adding TradeAccount record to DB. Exception: {ex}");
                        loggerException.Error(this.GetErrorLogMessage(ex));
                    }

                    counter++;
                }

                Console.WriteLine($"ImportJob: {newAccCounter} trade accounts have been added");
                logger.Log(LogLevel.Info, $"import$ImportJob: {newAccCounter} trade accounts have been added");
                Console.WriteLine($"ImportJob: {updatingAccCounter} trade accounts have been updated");
                logger.Log(LogLevel.Info, $"import$ImportJob: {updatingAccCounter} trade accounts have been updated");
            }

            _importJobRepository.SaveChanges();

            RefillTradeAccounts();

            GC.Collect();

            return await Task.FromResult(FileStatus.Success);
        }

        private void RefillTradeAccounts()
        {
            var accounts = _importJobRepository.TradeAccountsQuery().Select(acc => new {acc.AccountName, acc.Id})
                .ToList();
            accounts.ForEach(acc =>
            {
                if (!_tradeAccountModel.Contains(acc.AccountName))
                {
                    _tradeAccountModel.Add(acc.AccountName, new TradeAccountModel {Id = acc.Id});
                }
            });
        }
    }
}