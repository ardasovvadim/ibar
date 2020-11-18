using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IBAR.Api.Filters
{
    public class ParsePagination : ActionFilterAttribute
    {
        private const int zeroIndex = 0;
        private const int firstIndex = 1;

        public override void OnActionExecuting(HttpActionContext context)
        {
            var headers = context.Request.Headers;
            
            var paginIndex = 0;
            var paginSize = 20;
            var sorting = "";

            if (headers.Contains("Pagination") && headers.GetValues("Pagination").Any())
            {
                var paginationHeaderArgs = headers.GetValues("Pagination").FirstOrDefault().Split(';');
                paginIndex = int.Parse(paginationHeaderArgs[zeroIndex]);
                paginSize = int.Parse(paginationHeaderArgs[firstIndex]);
            }

            if (headers.Contains("Sorting") && headers.GetValues("Sorting").Any())
            {
                sorting = headers.GetValues("Sorting").FirstOrDefault();
            }
            
            context.ActionArguments.Add("pageIndex", paginIndex);
            context.ActionArguments.Add("paginSize", paginSize);
            context.ActionArguments.Add("sorting", sorting);
        }
    }
}