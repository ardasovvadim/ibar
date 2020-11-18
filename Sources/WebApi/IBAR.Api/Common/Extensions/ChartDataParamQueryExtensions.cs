using System;
using System.Globalization;
using IBAR.Api.Data;

namespace IBAR.Api.Common.Extensions
{
    public static class ChartDataParamQueryExtensions
    {
        public static DateTime ToStandardAppDateFormat(this string date)
        {
            return DateTime.ParseExact(date, "yyyy-dd-MM", CultureInfo.CurrentCulture.DateTimeFormat);
        }
    }
}