using System.Collections.Generic;
using System.Linq;
using IBAR.Api.Data;
using IBAR.TradeModel.Business.ViewModels.Request;

namespace IBAR.Api.Common.Extensions
{
    public static class PeriodStringExtension
    {
        public static Period ToPeriod(this PeriodString periodString)
        {
            return new Period
            {
                FromDate = periodString.StartDate.ToStandardAppDateFormat(),
                ToDate =  periodString.EndDate.ToStandardAppDateFormat().AddHours(23).AddMinutes(59).AddSeconds(59)
            };
        }

        public static IEnumerable<Period> ToPeriodList(this IEnumerable<PeriodString> periodStrings)
        {
            return periodStrings.Select(p => p.ToPeriod()).ToList();
        }
    }
}