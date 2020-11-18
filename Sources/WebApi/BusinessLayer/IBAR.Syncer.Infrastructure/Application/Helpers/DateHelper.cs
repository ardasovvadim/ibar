using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IBAR.Syncer.Infrastructure.Application.Helpers
{
    public static class DateHelper
    {
        public const string DefaultDateFormat = "yyyyMMdd";
        public const string DefaultDateTimeFormat = "yyyyMMddHHmmss";

        public static DateTime? ParseDate(string date, int positionDate = 0)
        {
            if (string.IsNullOrEmpty(date)) return null;
            
            var regexExpression = @"[\d]{" + DefaultDateFormat.Length + "}";

            var extract = Regex.Matches(date, regexExpression)[positionDate].Value;
            if (DateTime.TryParseExact(extract, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var resultDate))
            {
                return resultDate;
            }

            return null;
        }

        public static DateTime? ParseDateTime(string dateTime, params string[] customRegexExpressions)
        {
            if (string.IsNullOrEmpty(dateTime)) return null;

            if (customRegexExpressions.Length == 0)
            {
                customRegexExpressions = new[] {DefaultDateTimeFormat};
            }

            foreach (var expression in customRegexExpressions)
            {
                var regexExpression = @"^[\d\W]{" + expression.Length + "}$";
                if (Regex.IsMatch(dateTime, regexExpression))
                {
                    return DateTime.ParseExact(dateTime, expression, CultureInfo.InvariantCulture);
                }
            }

            return null;
        }
    }
}