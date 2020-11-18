using System;
using System.Collections.Generic;
using System.Globalization;

namespace IBAR.TradeModel.Business.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<int> EnumerateYears(this DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new InvalidOperationException();
            }

            var diff = end.Year - start.Year;
            int begin = start.Year;

            for (int i = 0; i <= diff; i++)
            {
                yield return begin++;
            }
        }

        public static string ToStandardAppDateFormatString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
        public static DateTime ToStandardAppDateFormat(this string date)
        {
            return DateTime.ParseExact(date, "yyyy-dd-MM", CultureInfo.CurrentCulture.DateTimeFormat);
        }
    }
}