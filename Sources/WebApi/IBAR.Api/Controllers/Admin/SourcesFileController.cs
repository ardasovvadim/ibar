using System.Net.Http;
using System.Web.Http;
using IBAR.Api.Common.Extensions;
using IBAR.Api.Data;
using IBAR.Api.Filters;
using IBAR.TradeModel.Business.Services;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles;

namespace IBAR.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/sources")]
    public class SourcesFileController : ApiController
    {
        private readonly ISourcesFilesService _sourcesFilesService;

        public SourcesFileController(ISourcesFilesService sourcesFilesService)
        {
            _sourcesFilesService = sourcesFilesService;
        }

        [HttpPost]
        [Route("GetSourcesFilesList")]
        [ParsePagination]
        public PaginationModel<SourceFileModel> GetSourcesFilesList([FromBody] SourceFileParamQuery sourceFileParamQuery)
        {
            int pageIndex = (int) ActionContext.ActionArguments["pageIndex"];
            int pageSize = (int) ActionContext.ActionArguments["paginSize"];
            string sorting = (string) ActionContext.ActionArguments["sorting"];
            Period period = null;
            if (sourceFileParamQuery.Period != null)
            {
                period = new Period
                {
                    FromDate = sourceFileParamQuery.Period.StartDate.ToStandardAppDateFormat(),
                    ToDate = sourceFileParamQuery.Period.EndDate.ToStandardAppDateFormat()
                };
            }

            return _sourcesFilesService.GetSourceFilesList(pageIndex, pageSize, sorting, period,
                sourceFileParamQuery.SearchName);
        }

        [HttpGet]
        [Route("DownloadSourceFile/{id}")]
        public HttpResponseMessage DownloadSourceFile(long id)
        {
            return _sourcesFilesService.DownloadSourceFile(id);
        }
    }
}