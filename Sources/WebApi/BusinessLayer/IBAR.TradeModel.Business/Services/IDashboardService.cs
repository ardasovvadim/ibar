using IBAR.TradeModel.Business.Data;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Dashboard;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace IBAR.TradeModel.Business.Services
{
    public interface IDashboardService
    {
        ChartDataVm LoadTotalIncome(IEnumerable<Period> periods, long[] idMasterAccounts, long[] idTradeAccounts);
        ChartDataVm LoadTotalClients(IEnumerable<Period> periods, long[] idMasterAccounts, long[] idTradeAccounts);
        ChartDataVm LoadTotalAum(IEnumerable<Period> periods, long[] idMasterAccounts, long[] idTradeAccounts);

        TotalDataVm<DashboardEnum> GetTotals(IEnumerable<DashboardEnum> toDashboardEnumTypes, Period period,
            IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
    }

    public class DashboardService : IDashboardService
    {
        private ITradeRepository _tradeRepository;

        public DashboardService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public ChartDataVm LoadTotalIncome(IEnumerable<Period> periodsParam, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            var periods = periodsParam.ToList();
            var startPeriods = periods.FirstOrDefault()?.FromDate;
            var endPeriods = periods.LastOrDefault()?.ToDate;
            var result = ChartDataVm.Default;

            if (startPeriods == null || endPeriods == null) return result;

            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();
            Dictionary<long, ChartDataVm.DataSet> data;

            var fees = _tradeRepository
                .TradeFeesQuery()
                .Include(f => f.TradeAccount.MasterAccount)
                .Where(s => s.ExternalDate >= startPeriods && s.ExternalDate <= endPeriods);

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                fees = fees.Where(fee => idTradeAccounts.Contains(fee.TradeAccountId));
                data = GetDictionaryByTradeAccounts(idTradeAccounts);

                periods.ForEach(period =>
                {
                    var extractedData = fees
                        .Where(fee => fee.ExternalDate >= period.FromDate && fee.ExternalDate <= period.ToDate)
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
                            Number = select.Sum(f => f.NetInBase)
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
                    fees = fees.Where(fee => idMasterAccounts.Contains(fee.TradeAccount.MasterAccountId));
                }

                periods.ForEach(period =>
                {
                    var extractedData = fees
                        .Where(fee => fee.ExternalDate >= period.FromDate && fee.ExternalDate <= period.ToDate)
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
                            Number = select.Sum(f => f.NetInBase)
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

        private Dictionary<long, ChartDataVm.DataSet> GetDictionaryByMasterAccounts(long[] idMasterAccounts)
        {
            var result = new Dictionary<long, ChartDataVm.DataSet>();

            var accountsQuery = _tradeRepository.MasterAccountsQuery();

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

            var accounts = _tradeRepository.TradeAccountsQuery()
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

        public ChartDataVm LoadTotalClients(IEnumerable<Period> periodsParam, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            var result = ChartDataVm.Default;
            var periods = periodsParam.ToList();
            var endPeriod = periods.LastOrDefault()?.ToDate;

            if (endPeriod == null) return result;

            Dictionary<long, ChartDataVm.DataSet> data;
            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();

            var accountsQuery = _tradeRepository
                .TradeAccountsQuery()
                .Include(a => a.MasterAccount)
                .Where(account => account.DateOpened != null &&
                                  account.DateOpened <= endPeriod &&
                                  (account.DateClosed == null ||
                                   (account.DateClosed != null && account.DateClosed > endPeriod)));

            data = GetDictionaryByMasterAccounts(idMasterAccounts);

            // sorting by selected master accounts
            if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                accountsQuery = accountsQuery.Where(s => idMasterAccounts.Contains(s.MasterAccountId));
            }

            periods.ForEach(period =>
            {
                var extractedAccountData = accountsQuery
                    .GroupBy(group => new
                    {
                        group.MasterAccount.Id,
                        group.MasterAccount.AccountName,
                        group.MasterAccount.AccountAlias
                    })
                    .Select(select => new ExtractedAccountData
                    {
                        Id = select.Key.Id,
                        AccountName = select.Key.AccountName,
                        AccountAlias = select.Key.AccountAlias,
                        Number = select.Count(account => account.DateOpened != null &&
                                                         account.DateOpened <= period.ToDate &&
                                                         (account.DateClosed == null ||
                                                          account.DateClosed != null &&
                                                          account.DateClosed > period.ToDate))
                    })
                    .ToList();

                ProcessExtractedData(extractedAccountData, data);
            });

            if (data.Any())
            {
                result.Data = data.Select(d => d.Value).ToArray();
            }

            return result;
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

        public ChartDataVm LoadTotalAum(IEnumerable<Period> periodsParam, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            var result = ChartDataVm.Default;
            var periods = periodsParam.ToList();
            var endPeriod = periods.LastOrDefault()?.ToDate;

            if (endPeriod == null) return result;

            result.Labels = TradeUtils.GetPeriodLabels(periods).ToArray();
            Dictionary<long, ChartDataVm.DataSet> data;

            var tradeNavsQuery = _tradeRepository.TradeNavsQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                data = GetDictionaryByTradeAccounts(idTradeAccounts);

                tradeNavsQuery = tradeNavsQuery.Where(nav => idTradeAccounts.Contains(nav.TradeAccountId));

                periods.ForEach(period =>
                {
                    var extractedData = tradeNavsQuery
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
                            Number = select.Sum(tradeNav => tradeNav.Total)
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
                    tradeNavsQuery = tradeNavsQuery.Where(nav =>
                        idMasterAccounts.Contains(nav.TradeAccount.MasterAccountId));
                }

                periods.ForEach(period =>
                {
                    var extractedData = tradeNavsQuery
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
                            Number = select.Sum(tradeNav => tradeNav.Total)
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

        private decimal LoadTotalIncomeTotal(Period period, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            decimal total;
            var fees = _tradeRepository
                .TradeFeesQuery()
                .Include(a => a.TradeAccount.MasterAccount)
                .Where(s => s.ExternalDate >= period.FromDate && s.ExternalDate <= period.ToDate);

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                total = fees.Where(fee => idTradeAccounts.Contains(fee.TradeAccountId)).Select(s => s.NetInBase)
                    .DefaultIfEmpty(0).Sum();
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                total = fees.Where(fee => idMasterAccounts.Contains(fee.TradeAccount.MasterAccount.Id)).Select(s => s.NetInBase)
                    .DefaultIfEmpty(0).Sum();
            }
            else
            {
                total = fees.Select(s => s.NetInBase).DefaultIfEmpty(0).Sum();
            }

            return total;
        }

        private decimal LoadTotalClientsTotal(Period period, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            var accounts = _tradeRepository
                .TradeAccountsQuery()
                .Include(acc => acc.MasterAccount)
                .Where(account => account.DateOpened != null &&
                                  account.DateOpened <= period.ToDate &&
                                  (account.DateClosed == null ||
                                   account.DateClosed != null && account.DateClosed > period.ToDate));

            return TradeUtils.IsSortingByMasterAccounts(idMasterAccounts)
                ? accounts.Count(account => idMasterAccounts.Contains(account.MasterAccountId))
                : accounts.Count();
        }

        private decimal LoadTotalAumTotal(Period period, long[] idMasterAccounts, long[] idTradeAccounts)
        {
            decimal total;
            var tradeNav = _tradeRepository.TradeNavsQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                total = tradeNav
                    .Where(s => DbFunctions.TruncateTime(s.ReportDate) == DbFunctions.TruncateTime(period.ToDate) &&
                                idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(t => t.Total).DefaultIfEmpty(0).Sum();
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                total = tradeNav
                    .Where(s => DbFunctions.TruncateTime(s.ReportDate) == DbFunctions.TruncateTime(period.ToDate) &&
                                idMasterAccounts.Contains(s.TradeAccount.MasterAccountId)).Select(t => t.Total)
                    .DefaultIfEmpty(0).Sum();
            }
            else
            {
                total = tradeNav
                    .Where(s => DbFunctions.TruncateTime(s.ReportDate) == DbFunctions.TruncateTime(period.ToDate))
                    .Select(t => t.Total).DefaultIfEmpty(0)
                    .Sum();
            }

            return total;
        }

        private decimal LoadAvgIncomeClient(Period period, long[] idMasterAccounts, long[] idTradeAccounts, IList<decimal> totals)
        {
            decimal total;
            var totalIncome = totals[(int) DashboardEnum.TotalIncome];
            var clientsTotal = totals[(int) DashboardEnum.TotalClients];

            clientsTotal = clientsTotal == 0 ? 1 : clientsTotal;
            total = totalIncome / clientsTotal;

            return total;
        }

        private decimal LoadAvgAccountSize(Period period, long[] idMasterAccounts, long[] idTradeAccounts, IList<decimal> totals)
        {
            decimal total;

            var aumTotal = totals[(int) DashboardEnum.TotalAum];
            var feesClientsTotal = totals[(int)DashboardEnum.TotalClients];
            
            feesClientsTotal = feesClientsTotal == 0 ? 1 : feesClientsTotal;
            total = aumTotal / feesClientsTotal;

            return total;
        }

        private decimal LoadAvgDailyIncome(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts, IList<decimal> totals)
        {
            decimal total;

            var busDays = Weekdays(period.FromDate, period.ToDate) == 0
                ? 1
                : Weekdays(period.FromDate, period.ToDate);

            var totalIncome = totals[(int) DashboardEnum.TotalIncome];
            
            total = totalIncome / busDays;

            return total;
        }

        private decimal LoadDeposits(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal total;
            var cash = _tradeRepository.TradeCashesQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                total = cash
                    .Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate &&
                                idTradeAccounts.Contains(s.TradeAccount.Id)).Select(t => t.Deposits).DefaultIfEmpty(0)
                    .Sum();
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                total = cash
                    .Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate &&
                                idMasterAccounts.Contains(s.TradeAccount.MasterAccountId)).Select(t => t.Deposits)
                    .DefaultIfEmpty(0).Sum();
            }
            else
            {
                total = cash.Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate)
                    .Select(t => t.Deposits).DefaultIfEmpty(0).Sum();
            }

            return total;
        }

        private decimal LoadWithdrawals(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal total;
            var cash = _tradeRepository.TradeCashesQuery();

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                total = cash
                    .Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate &&
                                idTradeAccounts.Contains(s.TradeAccount.Id)).Select(t => t.Withdrawals)
                    .DefaultIfEmpty(0).Sum();
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                total = cash
                    .Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate &&
                                idMasterAccounts.Contains(s.TradeAccount.MasterAccountId)).Select(t => t.Withdrawals)
                    .DefaultIfEmpty(0).Sum();
            }
            else
            {
                total = cash.Where(s => s.ReportDate <= period.ToDate && s.ReportDate >= period.FromDate)
                    .Select(t => t.Withdrawals).DefaultIfEmpty(0).Sum();
            }

            return total;
        }

        private decimal? LoadTotalStocks(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal? total;
            decimal? end;
            decimal? start;
            var tradeAs = _tradeRepository.TradeAsQuery().Where(t => t.AssetCategory == "STK");
            var endDate = period.ToDate.Date;
            var dateEnd = DateTime.ParseExact(endDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            var startDate = period.FromDate.Date == endDate ? DateTime.MinValue : period.FromDate.Date;
            var dateStart =
                DateTime.ParseExact(startDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);

            if (tradeAs.Where(s => s.ReportDate == dateEnd).Select(q => q.Quantity).Sum() == null)
            {
                var tempEndDate = tradeAs.Where(s => s.ReportDate <= dateEnd).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempEnd = tradeAs.Where(s => s.ReportDate >= dateEnd).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateEnd = tempEndDate == DateTime.MinValue ? tempEnd : tempEndDate;
            }

            if (tradeAs.Where(s => s.ReportDate == dateStart).Select(q => q.Quantity).Sum() == null &&
                startDate != DateTime.MinValue)
            {
                var tempStartDate = tradeAs.Where(s => s.ReportDate >= dateStart).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempStart = tradeAs.Where(s => s.ReportDate <= dateStart).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateStart = tempStart == DateTime.MinValue ? tempStartDate : tempStart;
            }

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs.Where(s => s.ReportDate == dateStart && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                end = tradeAs
                    .Where(s => s.ReportDate == dateEnd && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs
                    .Where(s => s.ReportDate == dateStart && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();

                start = tradeAs.Where(s => s.ReportDate == dateStart)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }

            return total;
        }

        private decimal? LoadTotalOptions(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal? total;
            decimal? end;
            decimal? start;
            var tradeAs = _tradeRepository.TradeAsQuery().Where(t => t.AssetCategory == "OPT");
            var endDate = period.ToDate.Date;
            var dateEnd = DateTime.ParseExact(endDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            var startDate = period.FromDate.Date == endDate ? DateTime.MinValue : period.FromDate.Date;
            var dateStart =
                DateTime.ParseExact(startDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);


            if (tradeAs.Where(s => s.ReportDate == dateEnd).Select(q => q.Quantity).Sum() == null)
            {
                var tempEndDate = tradeAs.Where(s => s.ReportDate <= dateEnd).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempEnd = tradeAs.Where(s => s.ReportDate >= dateEnd).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateEnd = tempEndDate == DateTime.MinValue ? tempEnd : tempEndDate;
            }

            if (tradeAs.Where(s => s.ReportDate == dateStart).Select(q => q.Quantity).Sum() == null &&
                startDate != DateTime.MinValue)
            {
                var tempStartDate = tradeAs.Where(s => s.ReportDate >= dateStart).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempStart = tradeAs.Where(s => s.ReportDate <= dateStart).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateStart = tempStart == DateTime.MinValue ? tempStartDate : tempStart;
            }

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs.Where(s => s.ReportDate == dateStart && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                end = tradeAs
                    .Where(s => s.ReportDate == dateEnd && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity)?.DefaultIfEmpty(0).Sum();
                start = tradeAs
                    .Where(s => s.ReportDate == dateStart && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs.Where(s => s.ReportDate == dateStart)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }

            return total;
        }

        private decimal? LoadTotalFutures(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal? total;
            decimal? end;
            decimal? start;
            var tradeAs = _tradeRepository.TradeAsQuery().Where(t => t.AssetCategory == "FUT");
            var endDate = period.ToDate.Date;
            var dateEnd = DateTime.ParseExact(endDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            var startDate = period.FromDate.Date == endDate ? DateTime.MinValue : period.FromDate.Date;
            var dateStart =
                DateTime.ParseExact(startDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);


            if (tradeAs.Where(s => s.ReportDate == dateEnd).Select(q => q.Quantity).Sum() == null)
            {
                var tempEndDate = tradeAs.Where(s => s.ReportDate <= dateEnd).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempEnd = tradeAs.Where(s => s.ReportDate >= dateEnd).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateEnd = tempEndDate == DateTime.MinValue ? tempEnd : tempEndDate;
            }

            if (tradeAs.Where(s => s.ReportDate == dateStart).Select(q => q.Quantity).Sum() == null &&
                startDate != DateTime.MinValue)
            {
                var tempStartDate = tradeAs.Where(s => s.ReportDate >= dateStart).OrderBy(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                var tempStart = tradeAs.Where(s => s.ReportDate <= dateStart).OrderByDescending(s => s.ReportDate)
                    .Select(q => q.ReportDate).FirstOrDefault();
                dateStart = tempStart == DateTime.MinValue ? tempStartDate : tempStart;
            }

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs.Where(s => s.ReportDate == dateStart && idTradeAccounts.Contains(s.TradeAccount.Id))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                end = tradeAs
                    .Where(s => s.ReportDate == dateEnd && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs
                    .Where(s => s.ReportDate == dateStart && idMasterAccounts.Contains(s.TradeAccount.MasterAccountId))
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }
            else
            {
                end = tradeAs.Where(s => s.ReportDate == dateEnd)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                start = tradeAs.Where(s => s.ReportDate == dateStart)
                    .Select(q => q.Quantity).DefaultIfEmpty(0).Sum();
                total = end - start;
            }

            return total;
        }

        private decimal LoadTotalNewClients(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            var feesClientsEnd = _tradeRepository
                .TradeAccountsQuery()
                .Where(fee => fee.DateFunded <= period.ToDate && fee.TradeStatus == "O");

            var feesClientsStart = _tradeRepository
                .TradeAccountsQuery()
                .Where(fee => fee.DateFunded <= period.FromDate && fee.TradeStatus == "O");

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                var end = feesClientsEnd.Count(fee => idTradeAccounts.Contains(fee.Id));
                var start = feesClientsStart.Count(fee => idTradeAccounts.Contains(fee.Id));
                return end - start;
            }

            if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                var end = feesClientsEnd.Count(fee => idMasterAccounts.Contains(fee.MasterAccountId));
                var start = feesClientsStart.Count(fee => idMasterAccounts.Contains(fee.MasterAccountId));
                return end - start;
            }

            return feesClientsEnd.Count() - feesClientsStart.Count();
        }

        private decimal LoadTotalAbandoment(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            var totalAbondmentEndPeriod = _tradeRepository
                .TradeAccountsQuery()
                .Where(fee => fee.DateClosed <= period.ToDate);

            var totalAbondmentStartPeriod = _tradeRepository
                .TradeAccountsQuery()
                .Where(fee => fee.DateClosed <= period.FromDate);

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                var endPeriod = totalAbondmentEndPeriod
                    .Where(fee => idTradeAccounts.Contains(fee.Id)).Count(c => c.TradeStatus == "C");

                var startPeriod = totalAbondmentStartPeriod
                    .Where(fee => idTradeAccounts.Contains(fee.Id)).Count(c => c.TradeStatus == "C");

                return endPeriod - startPeriod;
            }

            if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                var endPeriod = totalAbondmentEndPeriod
                    .Where(fee => idMasterAccounts.Contains(fee.MasterAccountId))
                    .Count(c => c.TradeStatus == "C");

                var startPeriod = totalAbondmentStartPeriod
                    .Where(fee => idMasterAccounts.Contains(fee.MasterAccountId))
                    .Count(c => c.TradeStatus == "C");

                return endPeriod - startPeriod;
            }

            var endPer = totalAbondmentEndPeriod.Count(c => c.TradeStatus == "C");
            var startPer = totalAbondmentStartPeriod.Count(c => c.TradeStatus == "C");
            return endPer - startPer;
        }

        private decimal LoadNetChangeInClients(Period period, long[] idMasterAccounts,
            long[] idTradeAccounts)
        {
            decimal total;
            var totalClients = LoadTotalNewClients(period, idMasterAccounts, idTradeAccounts);
            var totalAbondment = LoadTotalAbandoment(period, idMasterAccounts, idTradeAccounts);
            total = totalClients - totalAbondment;
            return total;
        }

        private static int Weekdays(DateTime dtmStart, DateTime dtmEnd)
        {
            // This function includes the start and end date in the count if they fall on a weekday
            int dowStart = ((int) dtmStart.DayOfWeek == 0 ? 7 : (int) dtmStart.DayOfWeek);
            int dowEnd = ((int) dtmEnd.DayOfWeek == 0 ? 7 : (int) dtmEnd.DayOfWeek);
            TimeSpan tSpan = dtmEnd - dtmStart;
            if (dowStart <= dowEnd)
            {
                return (((tSpan.Days / 7) * 5) + Math.Max((Math.Min((dowEnd + 1), 6) - dowStart), 0));
            }

            return (((tSpan.Days / 7) * 5) + Math.Min((dowEnd + 6) - Math.Min(dowStart, 6), 5));
        }


        public TotalDataVm<DashboardEnum> GetTotals(IEnumerable<DashboardEnum> toDashboardEnumTypes, Period period,
            IEnumerable<long> idMasterAccounts,
            IEnumerable<long> idTradeAccounts)
        {
            var totalData = new TotalDataVm<DashboardEnum>();
            var dashboardTypes =
                toDashboardEnumTypes == null ? new List<DashboardEnum>() : toDashboardEnumTypes.ToList();

            dashboardTypes.ForEach(type =>
            {
                switch (type)
                {
                    case DashboardEnum.TotalIncome:
                        totalData[(int) DashboardEnum.TotalIncome] = LoadTotalIncomeTotal(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.TotalClients:
                        totalData[(int) DashboardEnum.TotalClients] = LoadTotalClientsTotal(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.TotalAum:
                        totalData[(int) DashboardEnum.TotalAum] = LoadTotalAumTotal(period, idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.AvgIncomeClient:
                        totalData[(int) DashboardEnum.AvgIncomeClient] = LoadAvgIncomeClient(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray(),
                            totalData.Totals);
                        break;
                    case DashboardEnum.AvgAccountSize:
                        totalData[(int) DashboardEnum.AvgAccountSize] = LoadAvgAccountSize(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray(), totalData.Totals);
                        break;
                    case DashboardEnum.AvgDailyIncome:
                        totalData[(int) DashboardEnum.AvgDailyIncome] = LoadAvgDailyIncome(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray(), totalData.Totals);
                        break;
                    case DashboardEnum.Deposits:
                        totalData[(int) DashboardEnum.Deposits] = LoadDeposits(period, idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.Withdrawals:
                        totalData[(int) DashboardEnum.Withdrawals] = LoadWithdrawals(period, idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.DwBalance:
                        totalData[(int) DashboardEnum.DwBalance] =
                            totalData[(int) DashboardEnum.Deposits] - totalData[(int) DashboardEnum.Withdrawals];
                        break;
                    case DashboardEnum.TotalStocks:
                        totalData[(int) DashboardEnum.TotalStocks] = LoadTotalStocks(period,
                            idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray()) ?? 0;
                        break;
                    case DashboardEnum.TotalOptions:
                        totalData[(int) DashboardEnum.TotalOptions] = LoadTotalOptions(period,
                            idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray()) ?? 0;
                        break;
                    case DashboardEnum.TotalFutures:
                        totalData[(int) DashboardEnum.TotalFutures] = LoadTotalFutures(period,
                            idMasterAccounts.ToArray(),
                            idTradeAccounts.ToArray()) ?? 0;
                        break;
                    case DashboardEnum.TotalNewClients:
                        totalData[(int) DashboardEnum.TotalNewClients] = LoadTotalNewClients(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.TotalAbandoment:
                        totalData[(int) DashboardEnum.TotalAbandoment] = LoadTotalAbandoment(period,
                            idMasterAccounts.ToArray(), idTradeAccounts.ToArray());
                        break;
                    case DashboardEnum.NetChangeInClients:
                        totalData[(int) DashboardEnum.NetChangeInClients] =
                            totalData[(int) DashboardEnum.TotalNewClients] -
                            totalData[(int) DashboardEnum.TotalAbandoment];
                        break;
                }
            });
            return totalData;
        }
    }
}