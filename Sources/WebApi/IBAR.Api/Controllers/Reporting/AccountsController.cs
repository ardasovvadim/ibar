using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using IBAR.Api.Common.Extensions;
using IBAR.Api.Data;
using IBAR.Api.Filters;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Request.ClientsPage;
using IBAR.TradeModel.Business.ViewModels.Request.PortfolioPage;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Data.Entities;
using PeriodString = IBAR.Api.Data.PeriodString;

namespace IBAR.Api.Controllers.Reporting
{
    [Authorize]
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        private readonly ITradeAccountService _tradeAccountService;

        public AccountsController(ITradeAccountService tradeAccountService)
        {
            _tradeAccountService = tradeAccountService;
        }

        [HttpPost]
        [ParsePagination]
        [Route("all")]
        public IHttpActionResult GetAccountsInfo(ChartDataParamQuery paramQuery)
        {
            var pageIndex = (int) ActionContext.ActionArguments["pageIndex"];
            var pageSize = (int) ActionContext.ActionArguments["paginSize"];
            var sorting = (string) ActionContext.ActionArguments["sorting"];
            var period = paramQuery.Periods.FirstOrDefault()?.ToPeriod();
            
            return Ok(_tradeAccountService.GetAccountsInfo(pageIndex, pageSize, sorting, paramQuery.IdMasterAccounts,
                paramQuery.IdTradeAccounts, period));
        }

        [HttpPost]
        [Route("newclients")]
        public IHttpActionResult LoadNewClients([FromBody] PeriodString period)
        {
            return Ok(_tradeAccountService.LoadNewClients(period.ToPeriod()));
        }

        [HttpGet]
        [Route("account-info/{accountName}")]
        public IHttpActionResult GetAccountInfo(string accountName)
        {
            return Ok(_tradeAccountService.GetAccountInfo(accountName));
        }

        [HttpGet]
        [Route("TradingPermission")]
        public IHttpActionResult GetTradingPermissions()
        {
            return Ok(_tradeAccountService.GetTradingPermissions());
        }

        [HttpPost]
        [Route("trade-note")]
        public IHttpActionResult AddNewTradeAccountNote(TradeAccountNoteCreateEditModel model)
        {
            return Ok(_tradeAccountService.AddNewTradeAccountNote(model));
        }

        [HttpDelete]
        [Route("trade-note/{id}")]
        public IHttpActionResult DeleteTradeAccountNote(long id)
        {
            return Ok(_tradeAccountService.DeleteTradeAccountNote(id));
        }

        [HttpPut]
        [Route("trade-rank")]
        public IHttpActionResult ChangeTradeAccountRank(TradeAccountRankEditModel model)
        {
            return Ok(_tradeAccountService.ChangeTradeAccountRank(model));
        }


        [HttpPost]
        [Route("totals/get")]
        public List<string> GetTotalData(ClientTradesModel clientTradesTotals)
        {
            return _tradeAccountService.GetTotals(clientTradesTotals.ClientId, clientTradesTotals.Enums.ToDashboardEnumTypes<TotalAccountEnum>());
        }

        [HttpPost]
        [Route("table/data/get")]
        public List<TotalAccountTableModel> GetTableData([FromBody] ClientTradesModel clientTradesTotals)
        {
            return _tradeAccountService.GetTableData(clientTradesTotals.ClientId);
        }

        [HttpPost]
        [Route("forms/data/get")]
        public IHttpActionResult GetFormsData([FromBody] OpenPositionsParamQuery paramQuery)
        {
            return Ok(_tradeAccountService.GetFormsData(paramQuery));
        }

        [HttpPost]
        [Route("list/data/get")]
        public List<TotalAccountListModel> GetListData([FromBody] ClientTradesModel clientTradesTotals)
        {
            return _tradeAccountService.GetListData(clientTradesTotals.ClientId);
        }


        [HttpGet]
        [Route("trades/trades-dates/{accountName}")]
        public IEnumerable<DateTime?> GetAvailableTradesDates(string accountName)
        {
            return _tradeAccountService.GetAvailableTradesDates(accountName);
        }


        [HttpPost]
        [Route("id-name")]
        public IHttpActionResult GetNextListTradeAccountsIdName(TradeAccountIdNameParams @params)
        {
            var period = @params.Period.ToPeriod();
            return Ok(_tradeAccountService.GetTradeAccountsIdName(@params.PageIndex, @params.PageLength, @params.SortBy, @params.MasterAccounts, @params.TradeAccounts, period));
        }

        [HttpGet]
        [Route("portfolio/{accountName}")]
        public IHttpActionResult GetPortfolio(string accountName)
        {
            return Ok(_tradeAccountService.GetPortfolio(accountName));
        }

        [HttpPost]
        [Route("portfolio/chart-data")]
        public IHttpActionResult GetPortfolioChartData(PortfolioChartDataQueryParam queryParams)
        {
            return Ok(_tradeAccountService.GetPortfolioChartData(queryParams));
        }

        [HttpPost]
        [Route("portfolio/open-positions")]
        public IHttpActionResult GetPortfolioOpenPositions(OpenPositionsParamQuery paramQuery)
        {
            return Ok(_tradeAccountService.GetPortfolioOpenPositions(paramQuery));
        }

        [HttpPost]
        [Route("portfolio/totals")]
        public IHttpActionResult GetPortfolioTotals(PortfolioTotalsParamQuery paramQuery)
        {
            return Ok(_tradeAccountService.GetPortfolioTotals(paramQuery));
        }

        [HttpGet]
        [Route("portfolio/open-pos-dates/{accountName}")]
        public IEnumerable<DateTime> GetAvailableOpenPositionDates(string accountName)
        {
            return _tradeAccountService.GetAvailableOpenPositionDates(accountName);
        }
    }
}