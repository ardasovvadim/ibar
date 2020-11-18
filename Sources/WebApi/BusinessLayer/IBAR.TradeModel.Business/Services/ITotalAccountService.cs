using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Dashboard;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace IBAR.TradeModel.Business.Services
{
    public interface ITotalAccountService
    {
        TotalDataVm<TotalAccountEnum> GetTotals(IEnumerable<TotalAccountEnum> toDashboardEnumTypes, Period period,
            IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
        List<TotalAccountTableModel> GetTableData(IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
        List<TotalAccountListModel> GetListData(Period period, IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
        TotalAccountListModel FilterListData(TotalAccountListEnum elementNumber, string searchExpression, IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts);
    }

    public class TotalAccountService : ITotalAccountService
    {

        private ITradeRepository _tradeRepository;

        public TotalAccountService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public TotalDataVm<TotalAccountEnum> GetTotals(IEnumerable<TotalAccountEnum> toDashboardEnumTypes, Period period, IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts)
        {
            var totalData = new TotalDataVm<TotalAccountEnum>();
            var idMasterAccountsArr = idMasterAccounts.ToArray();
            var idTradeAccountsArr = idTradeAccounts.ToArray();
            toDashboardEnumTypes.ToList().ForEach(type =>
            {
                switch (type)
                {
                    case TotalAccountEnum.TotalClients:
                        totalData[(int)TotalAccountEnum.TotalClients] = NewClientsTableData("ALL", idMasterAccountsArr, idTradeAccountsArr, false);
                        break;
                    case TotalAccountEnum.NewClientsLd:
                        totalData[(int)TotalAccountEnum.NewClientsLd] = NewClientsTableData("LD", idMasterAccountsArr, idTradeAccountsArr, false);
                        break;
                    case TotalAccountEnum.NewClientsMtd:
                        totalData[(int)TotalAccountEnum.NewClientsMtd] = NewClientsTableData("MTD", idMasterAccountsArr, idTradeAccountsArr, false);
                        break;
                }
            });
            return totalData;
        }

        public List<TotalAccountTableModel> GetTableData(IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts)
        {
            var list = new List<TotalAccountTableModel>();
            var idMasterAccountsArr = idMasterAccounts.ToArray();
            var idTradeAccountsArr = idTradeAccounts.ToArray();
            list.Add(new TotalAccountTableModel()
            {
                RowName = "NEW CLIENTS (FINISHED APP)",
                LastDay = NewClientsTableData("LD",idMasterAccountsArr, idTradeAccountsArr, true),
                MTD = NewClientsTableData("MTD", idMasterAccountsArr, idTradeAccountsArr, true),
                LastMonth = NewClientsTableData("LM", idMasterAccountsArr, idTradeAccountsArr, true),
                AvgDailyMonth = NewClientsTableData("MAVG", idMasterAccountsArr, idTradeAccountsArr, true),
                AvgDailyYear = NewClientsTableData("12MAVG", idMasterAccountsArr, idTradeAccountsArr, true)
            });

            list.Add(new TotalAccountTableModel()
            {
                RowName = "NEW CLIENTS DEPOSITS",
                LastDay = NewClientsTableData("LD", idMasterAccountsArr, idTradeAccountsArr, false),
                MTD = NewClientsTableData("MTD", idMasterAccountsArr, idTradeAccountsArr, false),
                LastMonth = NewClientsTableData("LM", idMasterAccountsArr, idTradeAccountsArr, false),
                AvgDailyMonth = NewClientsTableData("MAVG", idMasterAccountsArr, idTradeAccountsArr, false),
                AvgDailyYear = NewClientsTableData("12MAVG", idMasterAccountsArr, idTradeAccountsArr, false)
            });
            return list;
        }

        public List<TotalAccountListModel> GetListData(Period period, IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts)
        {
            var list = new List<TotalAccountListModel>();
            var tradeAccounts = _tradeRepository
             .TradeAccountsQuery().Where(s => s.IsClientInfo == true);

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts.ToArray()))
            {
                tradeAccounts = tradeAccounts.Where(fee => idTradeAccounts.Contains(fee.Id));
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts.ToArray()))
            {
                tradeAccounts = tradeAccounts.Where(fee => idMasterAccounts.Contains(fee.MasterAccount.Id));
            }

            var city = RetriveListData(tradeAccounts, TotalAccountListEnum.CityList);

            var state = RetriveListData(tradeAccounts, TotalAccountListEnum.CountryList);

            var capabilities = RetriveListData(tradeAccounts, TotalAccountListEnum.AccountCapabilitiesList);

            var accType = RetriveListData(tradeAccounts, TotalAccountListEnum.AccountTypeList);

            var idEntity = RetriveListData(tradeAccounts, TotalAccountListEnum.IbEntityList);

            var currency = RetriveListData(tradeAccounts, TotalAccountListEnum.BaseCurrencyList);

            //adding to list 
            list.Add(city);
            list.Add(state);
            list.Add(capabilities);
            list.Add(accType);
            list.Add(idEntity);
            list.Add(currency);

            return list;

        }


        public TotalAccountListModel FilterListData(TotalAccountListEnum elementNumber, string searchExpression, IEnumerable<long> idMasterAccounts, IEnumerable<long> idTradeAccounts)
        {
            var tradeAccounts = _tradeRepository.TradeAccountsQuery().Where(s => s.IsClientInfo == true);

            if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts.ToArray()))
            {
                tradeAccounts = tradeAccounts.Where(fee => idTradeAccounts.Contains(fee.Id));
            }
            else if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts.ToArray()))
            {
                tradeAccounts = tradeAccounts.Where(fee => idMasterAccounts.Contains(fee.MasterAccount.Id));
            }

            var listModel = RetriveListData(tradeAccounts, elementNumber);
            listModel.AccountTotalList = string.IsNullOrEmpty(searchExpression) ? listModel.AccountTotalList : listModel.AccountTotalList.Where(s => s.Key.ToLower().Contains(searchExpression.ToLower())).ToList();
            return listModel;
        }

        private decimal NewClientsTableData(string findPeriod, long[] idMasterAccounts, long[] idTradeAccounts, bool dif)
        {
            var tradeAccounts = _tradeRepository
             .TradeAccountsQuery().Where(s => s.IsClientInfo == true);

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
                    tradeAccounts = dif ? tradeAccounts.Where(s => s.DateOpened >= endDate && s.DateOpened <= startDate) : tradeAccounts.Where(s => s.DateFunded >= endDate && s.DateFunded <= startDate);
                    return tradeAccounts.Count();
                case "MTD":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    tradeAccounts = dif ? tradeAccounts.Where(s => s.DateOpened <= endDate && s.DateOpened >= startDate) : tradeAccounts.Where(s => s.DateFunded <= endDate && s.DateFunded >= startDate);
                    return tradeAccounts.Count();
                case "LM":
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    var last = month.AddDays(-1);
                    tradeAccounts = dif ? tradeAccounts.Where(s => s.DateOpened <= last && s.DateOpened >= first) : tradeAccounts.Where(s => s.DateFunded <= last && s.DateFunded >= first);
                    return tradeAccounts.Count();
                case "MAVG":
                    today = DateTime.Today;
                    month = new DateTime(today.Year, today.Month, 1);
                    first = month.AddMonths(-1);
                    last = month.AddDays(-1);
                    var numberOfDays = (last - first).TotalDays;
                    tradeAccounts = dif ? tradeAccounts.Where(s => s.DateOpened <= last && s.DateOpened >= first) : tradeAccounts.Where(s => s.DateFunded <= last && s.DateFunded >= first);
                    return Math.Round(Convert.ToDecimal(tradeAccounts.Count() / numberOfDays), 2);
                case "12MAVG":
                    decimal daysLeft = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - DateTime.Now.DayOfYear;
                    var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    tradeAccounts = dif ? tradeAccounts.Where(s => s.DateOpened <= endDate && s.DateOpened >= firstDayOfYear) : tradeAccounts.Where(s => s.DateFunded <= endDate && s.DateFunded >= firstDayOfYear);
                    return Math.Round(tradeAccounts.Count() / daysLeft, 2);
                case "ALL":
                    tradeAccounts = tradeAccounts.Where(s => s.DateFunded != null);
                    return tradeAccounts.Count();
                default:
                    return 0;

            }
        }

        private TotalAccountListModel RetriveListData(IQueryable<TradeAccount> tradeAccounts, TotalAccountListEnum elementNumber)
        {
            var list = new List<KeyValuePair<string, long>>();

            switch (elementNumber)
            {
                case TotalAccountListEnum.CityList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.CityResidentialAddress)).GroupBy(s => s.CityResidentialAddress).Where(g => g.Count() > 1)
                    .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                case TotalAccountListEnum.CountryList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.StateResidentialAddress)).GroupBy(s => s.StateResidentialAddress).Where(g => g.Count() > 1)
                    .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                case TotalAccountListEnum.AccountCapabilitiesList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.AccountCapabilities)).GroupBy(s => s.AccountCapabilities).Where(g => g.Count() > 1)
                    .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                case TotalAccountListEnum.AccountTypeList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.AccountType)).GroupBy(s => s.AccountType).Where(g => g.Count() > 1)
                   .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                case TotalAccountListEnum.IbEntityList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.IbEntity)).GroupBy(s => s.IbEntity).Where(g => g.Count() > 1)
                    .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                case TotalAccountListEnum.BaseCurrencyList:
                    list = tradeAccounts.Where(s => !string.IsNullOrEmpty(s.Currency)).GroupBy(s => s.Currency).Where(g => g.Count() > 1)
                   .ToDictionary(s => s.Key, g => (long)g.Count()).ToList();
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };

                default:
                    return new TotalAccountListModel { Enum = elementNumber, AccountTotalList = list };
            }
        }


    }
}
