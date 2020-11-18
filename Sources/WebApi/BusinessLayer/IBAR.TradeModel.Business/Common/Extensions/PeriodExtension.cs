using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Request;

namespace IBAR.TradeModel.Business.Common.Extensions
{
    public static class PeriodStringExtension
    {
        public static Period ToPeriod(this PeriodString periodString)
        {
            return new Period
            {
                FromDate = periodString.FromDate.ToStandardAppDateFormat(),
                ToDate =  periodString.ToDate.ToStandardAppDateFormat().AddHours(23).AddMinutes(59).AddSeconds(59)
            };
        }

        public static IEnumerable<Period> ToPeriodList(this IEnumerable<PeriodString> periodStrings)
        {
            return periodStrings.Select(p => p.ToPeriod()).ToList();
        }
    }
}