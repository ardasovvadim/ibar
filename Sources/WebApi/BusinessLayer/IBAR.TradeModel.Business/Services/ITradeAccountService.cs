using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using IBAR.ServiceLayer.Common;
using IBAR.TradeModel.Business.Common.Extensions;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Request.ClientsPage;
using IBAR.TradeModel.Business.ViewModels.Request.PortfolioPage;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.AccountInfoPage;
using IBAR.TradeModel.Business.ViewModels.Response.ClientsPage;
using IBAR.TradeModel.Business.ViewModels.Response.Dashboard;
using IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles;
using IBAR.TradeModel.Business.ViewModels.Response.PortolioPage;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface ITradeAccountService
    {
        IEnumerable<TradeAccountModel> GetAllTradeAccounts();
        TradeAccountModel GetBySearchName(string searchName);
        PaginationModel<TradeAccountInfoGridViewModel> GetAccountsInfo(int pageIndex, int pageSize, string sorting, long[] idMasterAccounts, long[] idTradeAccounts, Period period);
        ChartDataVm LoadNewClients(Period period);
        AccountInfoGridVM GetAccountInfo(string accountName);
        IEnumerable<TradingPermissionsGridVM> GetTradingPermissions();
        long AddNewTradeAccountNote(TradeAccountNoteCreateEditModel model);
        long DeleteTradeAccountNote(long id);
        long ChangeTradeAccountRank(TradeAccountRankEditModel model);
        List<string> GetTotals(string clientId, IEnumerable<TotalAccountEnum> enums);
        IEnumerable<IdNameModel> GetTradeAccountsIdName(int pageIndex, int pageLength, string sortBy, IEnumerable<long> masterAccounts, IEnumerable<long> tradeAccounts, Period period);
        List<TotalAccountTableModel> GetTableData(string clientId);
        PaginationModel<TradesExeInformationVm> GetFormsData(OpenPositionsParamQuery paramQuery);
        List<TotalAccountListModel> GetListData(string clientId);
        PortfolioVm GetPortfolio(string accountName);
        ChartDataVm GetPortfolioChartData(PortfolioChartDataQueryParam queryParams);
        PaginationModel<OpenPositionVm> GetPortfolioOpenPositions(OpenPositionsParamQuery paramQuery);
        TotalDataVm<PortfolioChartType> GetPortfolioTotals(PortfolioTotalsParamQuery paramQuery);
        IEnumerable<DateTime> GetAvailableOpenPositionDates(string accountName);
        IEnumerable<DateTime?> GetAvailableTradesDates(string accountName);
    }

    public class TradeAccountService : ITradeAccountService
    {
        private readonly ITradeAccountRepository _tradeAccountRepository;
        private readonly IMapper _mapper;
        private readonly IMasterAccountRepository _masterAccountRepository;
        private readonly IIdentityService _identityService;
        private readonly IActionContextAccessor _actionContext;
        private readonly ITradeExeRepository _tradeExeRepository;

        public TradeAccountService(ITradeAccountRepository tradeAccountRepository, IMapper mapper,
            IMasterAccountRepository masterAccountRepository, IIdentityService identityService,
            ITradeExeRepository tradeExeRepository,
            IActionContextAccessor actionContext)
        {
            _tradeAccountRepository = tradeAccountRepository;
            _mapper = mapper;
            _masterAccountRepository = masterAccountRepository;
            _identityService = identityService;
            _actionContext = actionContext;
            _tradeExeRepository = tradeExeRepository;
        }

        public IEnumerable<TradeAccountModel> GetAllTradeAccounts()
        {
            return _tradeAccountRepository.GetAll().Select(ToTradeAccountModel);
        }

        private static TradeAccountModel ToTradeAccountModel(TradeAccount dto)
        {
            return new TradeAccountModel
            {
                Id = dto.Id,
                AccountName = dto.AccountName,
                AccountAlias = dto.AccountAlias
            };
        }

        public TradeAccountModel GetBySearchName(string searchName)
        {
            if (!Regex.IsMatch(searchName, Patterns.AccountName))
            {
                _actionContext.SetModelError("search", "Invalid search name format");
                _actionContext.ThrowIfModelInvalid();
            }

            var account = _tradeAccountRepository.GetBySearchName(searchName);

            if (account == null)
            {
                _actionContext.SetModelError("search", "Trade account not found");
                _actionContext.ThrowIfModelInvalid();
            }

            return _mapper.Map<TradeAccountModel>(account);
        }

        public PaginationModel<TradeAccountInfoGridViewModel> GetAccountsInfo(int pageIndex, int pageSize,
            string sorting, long[] idMasterAccounts, long[] idTradeAccounts, Period period)
        {
            var result = new PaginationModel<TradeAccountInfoGridViewModel>();

            var query = _tradeAccountRepository.Query().Where(acc => !acc.Deleted &&
                                                                     acc.IsClientInfo &&
                                                                     DbFunctions.TruncateTime(acc.DateFunded) <=
                                                                     DbFunctions.TruncateTime(period.ToDate));

            query = SortAccountsInfoQuery(query, sorting);

            if (TradeUtils.IsSortingByMasterAccounts(idMasterAccounts))
            {
                query = query.Where(acc => idMasterAccounts.Contains(acc.MasterAccountId));
            }
            else if (TradeUtils.IsSortingByTradeAccounts(idTradeAccounts))
            {
                query = query.Where(acc => idTradeAccounts.Contains(acc.Id));
            }

            var accounts = query.Skip(pageSize * pageIndex).Take(pageSize).ToList();

            result.Data = accounts.Select(acc => _mapper.Map<TradeAccountInfoGridViewModel>(acc)).ToList();

            result.DataLength = query.Count();

            return result;
        }

        public ChartDataVm LoadNewClients(Period period)
        {
            var result = ChartDataVm.Default;

            var masterAccounts = _masterAccountRepository.GetAll().ToList();

            result.Labels = masterAccounts.Select(TradeUtils.ResolveMasterAccountName).ToArray();

            var query = _tradeAccountRepository.Query().Where(acc => !acc.Deleted && acc.IsClientInfo &&
                                                                     DbFunctions.TruncateTime(acc.DateFunded) <=
                                                                     DbFunctions.TruncateTime(period.ToDate));

            var data = new List<decimal>();
            masterAccounts.ForEach(masterAcc => { data.Add(query.Count(acc => acc.MasterAccountId == masterAcc.Id)); });

            result.Data = new[] {new ChartDataVm.DataSet {Data = data}};

            return result;
        }

        public AccountInfoGridVM GetAccountInfo(string accountName)
        {
            var account = _tradeAccountRepository.Query()
                .Where(acc => !acc.Deleted && acc.IsClientInfo)
                .Include(acc => acc.TradingPermissions)
                .Include(acc => acc.MasterAccount)
                .Include(acc => acc.TradeAccountNotes)
                .FirstOrDefault(acc => acc.AccountName == accountName);

            var model = _mapper.Map<AccountInfoGridVM>(account);
            model.TradeAccountNotes = model.TradeAccountNotes.OrderByDescending(note => note.CreatedDate);

            return model;
        }

        public IEnumerable<TradingPermissionsGridVM> GetTradingPermissions()
        {
            return _mapper.Map<IEnumerable<TradingPermissionsGridVM>>(
                _tradeAccountRepository.GetAllTradingPermissions());
        }

        public long AddNewTradeAccountNote(TradeAccountNoteCreateEditModel model)
        {
            var dto = _mapper.Map<TradeAccountNote>(model);

            dto.CreatedById = _identityService.GetIdentityId();

            if (!_tradeAccountRepository.IsExistsTradeAccount(dto.TradeAccountId))
                throw new EntityNotFoundException("Trade account does not exists");

            return _tradeAccountRepository.AddNewTradeAccountNote(dto).Id;
        }

        public long DeleteTradeAccountNote(long id)
        {
            if (!_tradeAccountRepository.IsExistsTradeAccountNote(id))
                throw new EntityNotFoundException("Trade account not found");

            _tradeAccountRepository.DeleteTradeAccountNote(id);

            return id;
        }

        public long ChangeTradeAccountRank(TradeAccountRankEditModel model)
        {
            var dto = _mapper.Map<TradeAccount>(model);

            if (!_tradeAccountRepository.IsExistsTradeAccount(dto.Id))
                throw new EntityNotFoundException();

            return _tradeAccountRepository.ChangeTradeRank(dto).Id;
        }

        public IEnumerable<IdNameModel> GetTradeAccountsIdName(int pageIndex, int pageLength, string sortBy,
            IEnumerable<long> masterAccounts, IEnumerable<long> tradeAccounts, Period period)
        {
            var query = _tradeAccountRepository.Query().Where(acc => !acc.Deleted &&
                                                                     acc.IsClientInfo &&
                                                                     DbFunctions.TruncateTime(acc.DateFunded) <=
                                                                     DbFunctions.TruncateTime(period.ToDate));

            query = SortAccountsInfoQuery(query, sortBy);

            if (TradeUtils.IsSortingByMasterAccounts(masterAccounts.ToArray()))
            {
                query = query.Where(acc => masterAccounts.Contains(acc.MasterAccountId));
            }
            else if (TradeUtils.IsSortingByTradeAccounts(tradeAccounts.ToArray()))
            {
                query = query.Where(acc => tradeAccounts.Contains(acc.Id));
            }

            var accounts = query.Skip(pageLength * pageIndex).Take(pageLength).ToList();

            return accounts.Select(acc => new IdNameModel {Id = acc.Id, Name = acc.AccountName});
        }

        public PortfolioVm GetPortfolio(string accountName)
        {
            var result = new PortfolioVm();

            var tradeNav = _tradeAccountRepository
                .QueryTradeNav()
                .Include(acc => acc.TradeAccount)
                .OrderByDescending(acc => acc.ReportDate)
                .FirstOrDefault(acc => acc.TradeAccount.AccountName == accountName);

            if (tradeNav != null)
            {
                result = _mapper.Map<PortfolioVm>(tradeNav);
                result.AccountId = tradeNav.TradeAccount.Id;
                result.AccountName = tradeNav.TradeAccount.AccountName;
                result.Name = tradeNav.TradeAccount.Name;
            }

            return result;
        }

        public ChartDataVm GetPortfolioChartData(PortfolioChartDataQueryParam queryParams)
        {
            var result = ChartDataVm.Default;

            result.Labels = TradeUtils.GetPeriodLabels(queryParams.Periods.ToPeriodList()).ToArray();

            switch (queryParams.ChartType)
            {
                case PortfolioChartType.Nav:
                    result.Data = new[] {GetPortfolioNavChartData(queryParams)};
                    break;
                case PortfolioChartType.OpenPositions:
                    result.Data = new[] {GetPortfolioOpenPositionsChartData(queryParams)};
                    break;
                case PortfolioChartType.TotalUn:
                    result.Data = new[] {GetPortfolioTotalUnChartData(queryParams)};
                    break;
            }

            return result;
        }

        public PaginationModel<OpenPositionVm> GetPortfolioOpenPositions(OpenPositionsParamQuery paramQuery)
        {
            var result = new PaginationModel<OpenPositionVm>();

            var date = paramQuery.DateString.ToStandardAppDateFormat();

            var accountId = _tradeAccountRepository.GetBySearchName(paramQuery.AccountName).Id;

            var query = _tradeAccountRepository.QueryOpenPositions()
                .Where(op => op.TradeAccountId == accountId
                             && DbFunctions.TruncateTime(date) == DbFunctions.TruncateTime(op.ReportDate))
                .OrderBy(op => op.Id);

            var data = query.Skip(paramQuery.PageIndex * paramQuery.PageLength).Take(paramQuery.PageLength).ToList();

            result.Data = _mapper.Map<List<OpenPositionVm>>(data);
            result.DataLength = query.Count();

            return result;
        }

        public TotalDataVm<PortfolioChartType> GetPortfolioTotals(PortfolioTotalsParamQuery paramQuery)
        {
            var result = new TotalDataVm<PortfolioChartType>();

            var period = paramQuery.PeriodString.ToPeriod();
            var accountId = _tradeAccountRepository.GetBySearchName(paramQuery.AccountName).Id;

            result[(int) PortfolioChartType.Nav] = _tradeAccountRepository.QueryTradeNav()
                .Where(nav => nav.TradeAccountId == accountId &&
                              nav.ReportDate >= period.FromDate &&
                              nav.ReportDate < period.ToDate)
                .OrderByDescending(nav => nav.ReportDate)
                .FirstOrDefault()?.Total ?? 0;

            result[(int) PortfolioChartType.OpenPositions] = _tradeAccountRepository.QueryTradeNav()
                .Where(nav => nav.TradeAccountId == accountId &&
                              nav.ReportDate >= period.FromDate &&
                              nav.ReportDate < period.ToDate)
                .OrderByDescending(nav => nav.ReportDate)
                .Sum(nav => (decimal?) nav.StockLong +
                            (decimal?) nav.StockShort +
                            (decimal?) nav.OptionsLong +
                            (decimal?) nav.OptionsShort +
                            (decimal?) nav.CommoditiesLong +
                            (decimal?) nav.CommoditiesShort) ?? 0;

            result[(int) PortfolioChartType.TotalUn] = _tradeAccountRepository.QueryOpenPositions()
                .Where(nav => nav.TradeAccountId == accountId &&
                              nav.ReportDate >= period.FromDate &&
                              nav.ReportDate < period.ToDate)
                .OrderByDescending(nav => nav.ReportDate)
                .Sum(nav => (decimal?) nav.FifoPnlUnrealized) ?? 0;

            return result;
        }

        public IEnumerable<DateTime> GetAvailableOpenPositionDates(string accountName)
        {
            var accountId = _tradeAccountRepository.GetBySearchName(accountName).Id;

            return _tradeAccountRepository.QueryOpenPositions()
                .Where(op => op.TradeAccountId == accountId)
                .Select(op => op.ReportDate)
                .Distinct()
                .ToList();
        }

        private ChartDataVm.DataSet GetPortfolioTotalUnChartData(PortfolioChartDataQueryParam queryParams)
        {
            var result = new ChartDataVm.DataSet();

            var accountId = _tradeAccountRepository.Query()
                .FirstOrDefault(acc => acc.AccountName == queryParams.AccountName)?.Id;

            var query = _tradeAccountRepository.QueryOpenPositions();
            var periods = queryParams.Periods.ToPeriodList().ToList();

            periods.ForEach(period =>
            {
                var value = query.Where(nav => nav.TradeAccountId == accountId &&
                                               nav.ReportDate >= period.FromDate &&
                                               nav.ReportDate < period.ToDate)
                    .OrderByDescending(nav => nav.ReportDate)
                    .Sum(nav => (decimal?) nav.FifoPnlUnrealized) ?? 0;

                result.Data.Add(value);
            });

            return result;
        }

        private ChartDataVm.DataSet GetPortfolioOpenPositionsChartData(PortfolioChartDataQueryParam queryParams)
        {
            var result = new ChartDataVm.DataSet();

            var accountId = _tradeAccountRepository.Query()
                .FirstOrDefault(acc => acc.AccountName == queryParams.AccountName)?.Id;

            var query = _tradeAccountRepository.QueryTradeNav();
            var periods = queryParams.Periods.ToPeriodList().ToList();

            try
            {
                periods.ForEach(period =>
                {
                    var value = query.Where(nav => nav.TradeAccountId == accountId &&
                                                   nav.ReportDate >= period.FromDate &&
                                                   nav.ReportDate < period.ToDate)
                        .OrderByDescending(nav => nav.ReportDate)
                        .Sum(nav => (decimal?) nav.StockLong + (decimal?) nav.StockShort +
                                    (decimal?) nav.OptionsLong + (decimal?) nav.OptionsShort +
                                    (decimal?) nav.CommoditiesLong + (decimal?) nav.CommoditiesShort) ?? 0;

                    result.Data.Add(Math.Abs(value));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        private ChartDataVm.DataSet GetPortfolioNavChartData(PortfolioChartDataQueryParam queryParams)
        {
            var result = new ChartDataVm.DataSet();

            var accountId = _tradeAccountRepository.Query()
                .FirstOrDefault(acc => acc.AccountName == queryParams.AccountName)?.Id;

            var query = _tradeAccountRepository.QueryTradeNav();
            var periods = queryParams.Periods.ToPeriodList().ToList();

            periods.ForEach(period =>
            {
                var value = query.Where(nav => nav.TradeAccountId == accountId &&
                                               nav.ReportDate >= period.FromDate &&
                                               nav.ReportDate < period.ToDate)
                    .OrderByDescending(nav => nav.ReportDate)
                    .FirstOrDefault()?.Total;

                result.Data.Add(value ?? 0);
            });

            return result;
        }

        private static IOrderedQueryable<TradeAccount> SortAccountsInfoQuery(IQueryable<TradeAccount> query,
            string sorting)
        {
            var sort = sorting.Split(';');
            var descAsc = sort.LastOrDefault();

            switch (sort.FirstOrDefault())
            {
                case "accountName":
                    return descAsc == "desc"
                        ? query.OrderByDescending(acc => acc.Name)
                        : query.OrderBy(acc => acc.Name);
                case "city":
                    return descAsc == "desc"
                        ? query.OrderByDescending(acc => acc.City)
                        : query.OrderBy(acc => acc.City);
                case "dateFunded":
                    return descAsc == "desc"
                        ? query.OrderByDescending(acc => acc.DateFunded)
                        : query.OrderBy(acc => acc.DateFunded);
                default:
                    return query.OrderByDescending(acc => acc.DateFunded);
            }
        }

        public List<string> GetTotals(string clientId, IEnumerable<TotalAccountEnum> enums)
        {
            var totalData = new List<string>();
            var api = _tradeExeRepository.Query().Where(s => s.TradeAccount.AccountName == clientId)
                .Select(i => i.IsAPIOrder)?.Distinct().ToList();
            var lastTradeDate = _tradeExeRepository.Query().Where(s => s.TradeAccount.AccountName == clientId)
                .OrderByDescending(s => s.ReportDate).Select(s => s.ReportDate).FirstOrDefault().ToString();
            var status = RetrieveClientStatus(clientId);

            if (api.Contains("N") && api.Contains("Y")) api.Add("Combine");

            totalData.Add(status);
            totalData.Add(api.LastOrDefault() ?? "None");
            totalData.Add(lastTradeDate);


            return totalData;
        }

        private string RetrieveClientStatus(string clientId)
        {
            string status = "";
            var tradeStatus = _tradeExeRepository.Query().Where(s => s.TradeAccount.AccountName == clientId)
                                  ?.OrderByDescending(s => s.ReportDate).Select(s => s.ReportDate).Distinct().Count() ??
                              0;
            if (tradeStatus <= 1)
            {
                status = "Day Trader";
            }
            else if (tradeStatus < 14 && tradeStatus > 1)
            {
                status = "Swing";
            }
            else if (tradeStatus >= 14)
            {
                status = "Long Term";
            }

            return status;
        }

        public List<TotalAccountTableModel> GetTableData(string clientId)
        {
            var list = new List<TotalAccountTableModel>();

            var totalTrades = new TotalAccountTableModel()
            {
                RowName = "TOTAL TRADES",
                LastDay = NewClientsTableData("LD", clientId, null, true),
                MTD = NewClientsTableData("MTD", clientId, null, true),
                LastMonth = NewClientsTableData("LM", clientId, null, true),
                AvgDailyMonth = NewClientsTableData("MAVG", clientId, null, true),
                AvgDailyYear = NewClientsTableData("12MAVG", clientId, null, true)
            };

            var avgTradeSize = new TotalAccountTableModel()
            {
                RowName = "AVG TRADE SIZE",
                LastDay = NewClientsTableData("LD", clientId, null, false),
                MTD = NewClientsTableData("MTD", clientId, null, false),
                LastMonth = NewClientsTableData("LM", clientId, null, false),
                AvgDailyMonth = NewClientsTableData("MAVG", clientId, null, false),
                AvgDailyYear = NewClientsTableData("12MAVG", clientId, null, false)
            };

            var volumeQ = new TotalAccountTableModel()
            {
                RowName = "STOCKS VOLUME(Q)",
                LastDay = NewClientsTableData("LD", clientId, "STK", false),
                MTD = NewClientsTableData("MTD", clientId, "STK", false),
                LastMonth = NewClientsTableData("LM", clientId, "STK", false),
                AvgDailyMonth = NewClientsTableData("MAVG", clientId, "STK", false),
                AvgDailyYear = NewClientsTableData("12MAVG", clientId, "STK", false)
            };

            var optionsQ = new TotalAccountTableModel()
            {
                RowName = "OPTIONS VOLUME(Q)",
                LastDay = NewClientsTableData("LD", clientId, "OPT", false),
                MTD = NewClientsTableData("MTD", clientId, "OPT", false),
                LastMonth = NewClientsTableData("LM", clientId, "OPT", false),
                AvgDailyMonth = NewClientsTableData("MAVG", clientId, "OPT", false),
                AvgDailyYear = NewClientsTableData("12MAVG", clientId, "OPT", false)
            };

            var fururesQ = new TotalAccountTableModel()
            {
                RowName = "FUTURES VOLUME(Q)",
                LastDay = NewClientsTableData("LD", clientId, "FUT", false),
                MTD = NewClientsTableData("MTD", clientId, "FUT", false),
                LastMonth = NewClientsTableData("LM", clientId, "FUT", false),
                AvgDailyMonth = NewClientsTableData("MAVG", clientId, "FUT", false),
                AvgDailyYear = NewClientsTableData("12MAVG", clientId, "FUT", false)
            };

            list.Add(totalTrades);
            list.Add(avgTradeSize);
            list.Add(volumeQ);
            list.Add(optionsQ);
            list.Add(fururesQ);
            return list;
        }

        private decimal NewClientsTableData(string findPeriod, string clientId, string assetCategory, bool isTrades)
        {
            var tradeAccounts = _tradeExeRepository
                .Query().Where(s => s.TradeAccount.AccountName == clientId);

            if (!string.IsNullOrEmpty(assetCategory))
                tradeAccounts = tradeAccounts.Where(s => s.AssetCategory == assetCategory);

            switch (findPeriod)
            {
                case "LD":
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    var endDate =
                        new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(-2);
                    if (isTrades)
                    {
                        var summ = tradeAccounts.Count(s => s.ReportDate >= endDate && s.ReportDate <= startDate);
                        return summ;
                    }

                    var sum = tradeAccounts.Where(s => s.ReportDate >= endDate && s.ReportDate <= startDate)
                        .Select(q => q.Quantity).DefaultIfEmpty(0).Sum() ?? 0;
                    return Math.Round(sum, 2);
                case "MTD":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    if (isTrades)
                    {
                        var summ = tradeAccounts.Count(s => s.ReportDate <= endDate && s.ReportDate >= startDate);
                        return summ;
                    }

                    sum = tradeAccounts.Where(s => s.ReportDate <= endDate && s.ReportDate >= startDate)
                        .Select(q => q.Quantity).DefaultIfEmpty(0).Sum() ?? 0;
                    return Math.Round(sum, 2);
                case "LM":
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    var last = month.AddDays(-1);
                    if (isTrades)
                    {
                        var summ = tradeAccounts.Count(s => s.ReportDate <= last && s.ReportDate >= first);
                        return summ;
                    }

                    sum = tradeAccounts.Where(s => s.ReportDate <= last && s.ReportDate >= first)
                        .Select(q => q.Quantity).DefaultIfEmpty(0).Sum() ?? 0;
                    return Math.Round(sum, 2);
                case "MAVG":
                    today = DateTime.Today;
                    month = new DateTime(today.Year, today.Month, 1);
                    first = month.AddMonths(-1);
                    last = month.AddDays(-1);
                    decimal numberOfDays = (decimal) (last - first).TotalDays;
                    if (isTrades)
                    {
                        var summ = tradeAccounts.Count(s => s.ReportDate <= last && s.ReportDate >= first);
                        return summ;
                    }

                    sum = tradeAccounts.Where(s => s.ReportDate <= last && s.ReportDate >= first)
                        .Select(q => q.Quantity).DefaultIfEmpty(0).Sum() ?? 0;
                    return Math.Round(sum / numberOfDays, 2);
                case "12MAVG":
                    decimal daysLeft = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - DateTime.Now.DayOfYear;
                    var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    if (isTrades)
                    {
                        var summ = tradeAccounts.Count(s => s.ReportDate <= endDate && s.ReportDate >= firstDayOfYear);
                        return Math.Round(summ / daysLeft, 2);
                    }

                    sum = tradeAccounts.Where(s => s.ReportDate <= endDate && s.ReportDate >= firstDayOfYear)
                        .Select(q => q.Quantity).DefaultIfEmpty(0).Sum() ?? 0;
                    return Math.Round(sum / daysLeft, 2);
                default:
                    return 0;
            }
        }

        public PaginationModel<TradesExeInformationVm> GetFormsData(OpenPositionsParamQuery paramQuery)
        {

            var result = new PaginationModel<TradesExeInformationVm>();

            var date = paramQuery.DateString.ToStandardAppDateFormat();

            var accountId = _tradeAccountRepository.GetBySearchName(paramQuery.AccountName).Id;

            var query = _tradeExeRepository.Query()
                .Where(op => op.TradeAccountId == accountId
                             && DbFunctions.TruncateTime(date) == DbFunctions.TruncateTime(op.ReportDate))
                .OrderBy(op => op.Id);

            var data = query.Skip(paramQuery.PageIndex * paramQuery.PageLength).Take(paramQuery.PageLength).ToList();

            result.Data = _mapper.Map<List<TradesExeInformationVm>>(data);
            result.DataLength = query.Count();

            return result;
        }

        public List<TotalAccountListModel> GetListData(string clientId)
        {
            var list = new List<TotalAccountListModel>();
            var tradeAccounts = _tradeExeRepository
                .Query().Where(s => s.TradeAccount.AccountName == clientId);

            var city = RetriveListData(tradeAccounts, 0);

            var state = RetriveListData(tradeAccounts, 1);

            list.Add(city);
            list.Add(state);

            return list;
        }

        private TotalAccountListModel RetriveListData(IQueryable<TradesExe> tradeAccounts, int elementNumber)
        {
            var list = new List<KeyValuePair<string, long>>();

            switch (elementNumber)
            {
                case 0:
                    var quantTradePrice = tradeAccounts.OrderByDescending(s => s.Quantity)
                        .GroupBy(group => new {group.IbOrderID, group.ReportDate, group.TradePrice, group.Quantity})
                        .ToList().Distinct();
                    // TODO: Tried to distinct
                    quantTradePrice = quantTradePrice.GroupBy(group => group.Key.IbOrderID).Select(x => x.First());
                    var dict = quantTradePrice
                        .ToDictionary(s => s.Key.IbOrderID.ToString(), y => (long) (y.Key.Quantity * y.Key.TradePrice))
                        .OrderByDescending(s => s.Value).ToList();
                    return new TotalAccountListModel {AccountTotalList = dict};

                case 1:
                    var quant = tradeAccounts.OrderByDescending(s => s.Quantity)
                        .GroupBy(group => new {group.IbOrderID, group.ReportDate, group.Quantity}).ToList().Distinct();
                    quant = quant.GroupBy(group => group.Key.IbOrderID).Select(x => x.First());
                    var diction = quant.ToDictionary(s => s.Key.IbOrderID.ToString(), y => (long) (y.Key.Quantity))
                        .OrderByDescending(s => s.Value).ToList();
                    return new TotalAccountListModel {AccountTotalList = diction};

                default:
                    return new TotalAccountListModel {AccountTotalList = list};
            }
        }

        public IEnumerable<DateTime?> GetAvailableTradesDates(string accountName)
        {

            var accountId = _tradeAccountRepository.GetBySearchName(accountName).Id;
            return _tradeExeRepository
                .Query()
                .Where(op => op.TradeAccountId == accountId)
                .Select(op => op.ReportDate)
                .Distinct()
                .ToList();
        }
    }
}