using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.Services
{
    public interface IIncomeService
    {
        List<TotalAccountTableModel> GetTableData(IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
        ChartDataVm LoadTotalIncome(IEnumerable<Period> enumerable, long[] idMasterAccounts, long[] idTradeAccounts);
        ChartDataVm LoadTradingCommissions(IEnumerable<Period> enumerable, long[] idMasterAccounts, long[] idTradeAccounts);
        ChartDataVm LoadInterest(IEnumerable<Period> enumerable, long[] idMasterAccounts, long[] idTradeAccounts);
        ChartDataVm LoadBorrowing(IEnumerable<Period> enumerable, long[] idMasterAccounts, long[] idTradeAccounts);
    }


    public class IncomeService : IIncomeService
    {
        private readonly ITradeRepository _tradeAccountRepository;

        public IncomeService(ITradeRepository tradeAccountRepository)
        {
            _tradeAccountRepository = tradeAccountRepository;
        }

        public List<TotalAccountTableModel> GetTableData(IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts)
        {
            var list = new List<TotalAccountTableModel>();
            var idMasterAccountsArr = idMasterAccounts.ToArray();
            var idTradeAccountsArr = idTradeAccounts.ToArray();
            var borrowing = new TotalAccountTableModel()
            {
                RowName = "BORROWING",
                LastDay = 0,
                MTD = 0,
                LastMonth = 0,
                AvgDailyMonth = 0,
                AvgDailyYear = 0
            };

            var tradingComissions = new TotalAccountTableModel()
            {
                RowName = "TRADING COMMISSIONS",
                LastDay = NewClientsTableData("LD", idMasterAccountsArr, idTradeAccountsArr, true),
                MTD = NewClientsTableData("MTD", idMasterAccountsArr, idTradeAccountsArr, true),
                LastMonth = NewClientsTableData("LM", idMasterAccountsArr, idTradeAccountsArr, true),
                AvgDailyMonth = NewClientsTableData("MAVG", idMasterAccountsArr, idTradeAccountsArr, true),
                AvgDailyYear = NewClientsTableData("12MAVG", idMasterAccountsArr, idTradeAccountsArr, true)
            };

            var interest = new TotalAccountTableModel()
            {
                RowName = "INTEREST",
                LastDay = NewClientsTableData("LD", idMasterAccountsArr, idTradeAccountsArr, false),
                MTD = NewClientsTableData("MTD", idMasterAccountsArr, idTradeAccountsArr, false),
                LastMonth = NewClientsTableData("LM", idMasterAccountsArr, idTradeAccountsArr, false),
                AvgDailyMonth = NewClientsTableData("MAVG", idMasterAccountsArr, idTradeAccountsArr, false),
                AvgDailyYear = NewClientsTableData("12MAVG", idMasterAccountsArr, idTradeAccountsArr, false)
            };

            var totalIncome = new TotalAccountTableModel()
            {
                RowName = "TOTAL INCOME",
                LastDay = borrowing.LastDay + tradingComissions.LastDay + interest.LastDay,
                MTD = borrowing.MTD + tradingComissions.MTD + interest.MTD,
                LastMonth = borrowing.LastMonth + tradingComissions.LastMonth + interest.LastMonth,
                AvgDailyMonth = borrowing.AvgDailyMonth + tradingComissions.AvgDailyMonth + interest.AvgDailyMonth,
                AvgDailyYear = borrowing.AvgDailyYear + tradingComissions.AvgDailyYear + interest.AvgDailyYear
            };

            list.Add(totalIncome);
            list.Add(tradingComissions);
            list.Add(interest);
            list.Add(borrowing);

            return list;
        }

        public ChartDataVm LoadBorrowing(IEnumerable<Period> periodsParam, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            return new ChartDataVm();
        }

        public ChartDataVm LoadInterest(IEnumerable<Period> periodsParam, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            var result = ChartDataVm.Default;
            var periods = periodsParam.ToList();
            var endPeriod = periods.LastOrDefault()?.ToDate;

            if (endPeriod == null) return result;

            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();
            Dictionary<long, ChartDataVm.DataSet> data;

            var tradeInterestAccruaQuery = _tradeAccountRepository.TradeInterestAccrua();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                data = GetDictionaryByTradeAccounts(idTradeAccounts);

                tradeInterestAccruaQuery = tradeInterestAccruaQuery.Where(nav => idTradeAccounts.Contains(nav.TradeAccountId));

                periods.ForEach(period =>
                {
                    var extractedData = tradeInterestAccruaQuery
                        .Where(tradeNav =>
                            DbFunctions.TruncateTime(tradeNav.ReportDate) == DbFunctions.TruncateTime(period.ToDate))
                        .GroupBy(group => new
                        {
                            group.TradeAccount.Id,
                            group.TradeAccount.AccountName,
                            group.TradeAccount.AccountAlias,
                        })
                        .Select(select => new ExtractedAccountData
                        {
                            Id = select.Key.Id,
                            AccountName = select.Key.AccountName,
                            AccountAlias = select.Key.AccountAlias,
                            Number = select.Sum(tradeInterest => tradeInterest.EndingAccrualBalance)
                        })
                        .ToList();

                    ProcessExtractedData(extractedData, data);
                });
            }
            else
            {
                data = GetDictionaryByMasterAccounts(idMasterAccounts);

                if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
                {
                    tradeInterestAccruaQuery = tradeInterestAccruaQuery.Where(nav =>
                        idMasterAccounts.Contains(nav.TradeAccount.MasterAccountId));
                }

                periods.ForEach(period =>
                {
                    var extractedData = tradeInterestAccruaQuery
                        .Where(tradeNav =>
                            DbFunctions.TruncateTime(tradeNav.ReportDate) == DbFunctions.TruncateTime(period.ToDate))
                        .GroupBy(group => new
                        {
                            group.TradeAccount.MasterAccount.Id,
                            group.TradeAccount.MasterAccount.AccountName,
                            group.TradeAccount.MasterAccount.AccountAlias
                        })
                        .Select(select => new ExtractedAccountData
                        {
                            Id = select.Key.Id,
                            AccountName = select.Key.AccountName,
                            AccountAlias = select.Key.AccountAlias,
                            Number = select.Sum(tradeInterest => tradeInterest.EndingAccrualBalance)
                        })
                        .ToList();

                    ProcessExtractedData(extractedData, data);
                });
            }

            if (data.Any())
            {
                result.Data = data.Select(d => d.Value).ToArray();
            }

            return result;
        }

        public ChartDataVm LoadTradingCommissions(IEnumerable<Period> periodsParam, long[] idMasterAccounts, long[] idTradeAccounts)
        {

            var result = ChartDataVm.Default;
            var periods = periodsParam.ToList();
            var endPeriod = periods.LastOrDefault()?.ToDate;

            if (endPeriod == null) return result;

            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();
            Dictionary<long, ChartDataVm.DataSet> data;

            var tradeCommissionsQuery = _tradeAccountRepository.TradeCommissionsQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                data = GetDictionaryByTradeAccounts(idTradeAccounts);

                tradeCommissionsQuery = tradeCommissionsQuery.Where(nav => idTradeAccounts.Contains(nav.TradeAccountId));

                periods.ForEach(period =>
                {
                    var extractedData = tradeCommissionsQuery
                        .Where(tradeNav =>
                            DbFunctions.TruncateTime(tradeNav.ReportDate) == DbFunctions.TruncateTime(period.ToDate))
                        .GroupBy(group => new
                        {
                            group.TradeAccount.Id,
                            group.TradeAccount.AccountName,
                            group.TradeAccount.AccountAlias,
                        })
                        .Select(select => new ExtractedAccountData
                        {
                            Id = select.Key.Id,
                            AccountName = select.Key.AccountName,
                            AccountAlias = select.Key.AccountAlias,
                            Number = select.Sum(commission => commission.FxRateToBase * commission.TotalCommission)
                        })
                        .ToList();

                    ProcessExtractedData(extractedData, data);
                });
            }
            else
            {
                data = GetDictionaryByMasterAccounts(idMasterAccounts);

                if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
                {
                    tradeCommissionsQuery = tradeCommissionsQuery.Where(nav =>
                        idMasterAccounts.Contains(nav.TradeAccount.MasterAccountId));
                }

                periods.ForEach(period =>
                {
                    var extractedData = tradeCommissionsQuery
                        .Where(tradeNav =>
                            DbFunctions.TruncateTime(tradeNav.ReportDate) == DbFunctions.TruncateTime(period.ToDate))
                        .GroupBy(group => new
                        {
                            group.TradeAccount.MasterAccount.Id,
                            group.TradeAccount.MasterAccount.AccountName,
                            group.TradeAccount.MasterAccount.AccountAlias
                        })
                        .Select(select => new ExtractedAccountData
                        {
                            Id = select.Key.Id,
                            AccountName = select.Key.AccountName,
                            AccountAlias = select.Key.AccountAlias,
                            Number = select.Sum(tradeNav => tradeNav.FxRateToBase * tradeNav.TotalCommission)
                        })
                        .ToList();

                    ProcessExtractedData(extractedData, data);
                });
            }

            if (data.Any())
            {
                result.Data = data.Select(d => d.Value).ToArray();
            }

            return result;
        }

        public ChartDataVm LoadTotalIncome(IEnumerable<Period> periodsParam, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            return new ChartDataVm();
        }

        private decimal NewClientsTableData(string findPeriod, long[] idMasterAccounts, long[] idTradeAccounts, bool dif)
        {
            var tradeAccounts = _tradeAccountRepository
             .TradeAccountsQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                tradeAccounts = tradeAccounts.Where(fee => idTradeAccounts.Contains(fee.Id));
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                tradeAccounts = tradeAccounts.Where(fee => idMasterAccounts.Contains(fee.MasterAccount.Id));
            }

            switch (findPeriod)
            {
                case "LD":
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(-1);
                    decimal res = dif ? tradeAccounts.Where(s => s.DateOpened >= endDate && s.DateOpened <= startDate).Select(s => s.TradeFees.Select(sum => sum.NetInBase).DefaultIfEmpty(0).Sum()).FirstOrDefault() : tradeAccounts.Where(s => s.DateFunded >= endDate && s.DateFunded <= startDate).Select(s => s.TradeInterestAccruas.Select(sum => sum.EndingAccrualBalance).DefaultIfEmpty(0).Sum()).FirstOrDefault();
                    return Math.Round(res, 2);
                case "MTD":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    res = dif ? tradeAccounts
                        .Where(s => s.DateOpened <= endDate && s.DateOpened >= startDate)
                        .Select(s => s.TradeFees.Select(sum => sum.NetInBase)
                            .DefaultIfEmpty(0).Sum()).FirstOrDefault() : 
                        tradeAccounts.Where(s => s.DateFunded <= endDate && s.DateFunded >= startDate)
                            .Select(s => s.TradeInterestAccruas
                                .Select(sum => sum.EndingAccrualBalance)
                                .DefaultIfEmpty(0).Sum()).FirstOrDefault();
                    return Math.Round(res, 2);
                case "LM":
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    var last = month.AddDays(-1);
                    res = dif ? tradeAccounts.Where(s => s.DateOpened <= last && s.DateOpened >= first).Select(s => s.TradeFees.Select(sum => sum.NetInBase).DefaultIfEmpty(0).Sum()).FirstOrDefault() : tradeAccounts.Where(s => s.DateFunded <= last && s.DateFunded >= first).Select(s => s.TradeInterestAccruas.Select(sum => sum.EndingAccrualBalance).DefaultIfEmpty(0).Sum()).FirstOrDefault();
                    return Math.Round(res, 2);
                case "MAVG":
                    today = DateTime.Today;
                    month = new DateTime(today.Year, today.Month, 1);
                    first = month.AddMonths(-1);
                    last = month.AddDays(-1);
                    int numberOfDays = (int)(last - first).TotalDays;
                    res = dif ? tradeAccounts.Where(s => s.DateOpened <= last && s.DateOpened >= first).Select(s => s.TradeFees.Select(sum => sum.NetInBase).DefaultIfEmpty(0).Sum()).FirstOrDefault() : tradeAccounts.Where(s => s.DateFunded <= last && s.DateFunded >= first).Select(s => s.TradeInterestAccruas.Select(sum => sum.EndingAccrualBalance).DefaultIfEmpty(0).Sum()).FirstOrDefault();
                    return Math.Round(Convert.ToDecimal(res / numberOfDays), 2);
                case "12MAVG":
                    decimal daysLeft = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - DateTime.Now.DayOfYear;
                    var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    res = dif ? tradeAccounts.Where(s => s.DateOpened <= endDate && s.DateOpened >= firstDayOfYear).Select(s => s.TradeFees.Select(sum => sum.NetInBase).DefaultIfEmpty(0).Sum()).FirstOrDefault() : tradeAccounts.Where(s => s.DateFunded <= endDate && s.DateFunded >= firstDayOfYear).Select(s => s.TradeInterestAccruas.Select(sum => sum.EndingAccrualBalance).DefaultIfEmpty(0).Sum()).FirstOrDefault();
                    return Math.Round(res / daysLeft, 2);
                default:
                    return 0;

            }
        }

        private void ProcessExtractedData(List<ExtractedAccountData> extractedData,
         Dictionary<long, ChartDataVm.DataSet> data)
        {
            var forFillZero = data.Keys.ToList();

            extractedData.ForEach(account =>
            {
                if (!data.ContainsKey(account.Id))
                {
                    data[account.Id] = new ChartDataVm.DataSet
                    {
                        Label = string.IsNullOrEmpty(account.AccountAlias)
                            ? account.AccountName
                            : account.AccountAlias
                    };
                }

                data[account.Id].Data.Add(account.Number);

                forFillZero.Remove(account.Id);
            });

            forFillZero.ForEach(accountId => { data[accountId].Data.Add(0); });
        }

        private Dictionary<long, ChartDataVm.DataSet> GetDictionaryByMasterAccounts(long[] idMasterAccounts)
        {
            var result = new Dictionary<long, ChartDataVm.DataSet>();

            var accountsQuery = _tradeAccountRepository.MasterAccountsQuery();

            if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                accountsQuery = accountsQuery.Where(account => idMasterAccounts.Contains(account.Id));
            }

            var accounts = accountsQuery.Select(account => new ExtractedAccountData
            {
                Id = account.Id,
                AccountName = account.AccountName,
                AccountAlias = account.AccountAlias
            }).ToList();

            accounts.ForEach(account =>
            {
                result.Add(account.Id, new ChartDataVm.DataSet());
                result[account.Id].Label = string.IsNullOrEmpty(account.AccountAlias)
                    ? account.AccountName
                    : account.AccountAlias;
            });

            return result;
        }

        private Dictionary<long, ChartDataVm.DataSet> GetDictionaryByTradeAccounts(long[] idTradeAccounts)
        {
            var result = new Dictionary<long, ChartDataVm.DataSet>();

            var accounts = _tradeAccountRepository.TradeAccountsQuery()
                .Where(account => idTradeAccounts.Contains(account.Id))
                .Select(account => new ExtractedAccountData
                {
                    Id = account.Id,
                    AccountName = account.AccountName,
                    AccountAlias = account.AccountAlias
                })
                .ToList();

            accounts.ForEach(account =>
            {
                result.Add(account.Id, new ChartDataVm.DataSet());
                result[account.Id].Label = string.IsNullOrEmpty(account.AccountAlias)
                    ? account.AccountName
                    : account.AccountAlias;
            });

            return result;
        }

    }
}
