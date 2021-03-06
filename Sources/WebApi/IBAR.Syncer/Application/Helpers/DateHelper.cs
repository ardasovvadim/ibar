using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IBAR.Syncer.Application.Helpers
{
    public static class DateHelper
    {
        public const string DefaultDateFormat = "yyyyMMdd";
        public const string DefaultDateTimeFormat = "yyyyMMddHHmmss";

        public static DateTime? ParseDate(string fileName, int positionDate = 0)
        {
            var regexExpression = @"[\d]{" + DefaultDateFormat.Length + "}";

            var extract = Regex.Matches(fileName, regexExpression)[positionDate].Value;
            if (DateTime.TryParseExact(extract, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var resultDate))
            {
                return resultDate;
            }

            return null;
        }

        public static DateTime? ParseDateTime(string strDateTime, params string[] customRegexExpressions)
        {
            if (strDateTime == null) return null;

            if (customRegexExpressions.Length == 0)
            {
                customRegexExpressions = new[] {DefaultDateTimeFormat};
            }

            foreach (var expression in customRegexExpressions)
            {
                var regexExpression = @"^[\d\W]{" + expression.Length + "}$";
                if (Regex.IsMatch(strDateTime, regexExpression))
                {
                    return DateTime.ParseExact(strDateTime, expression, CultureInfo.InvariantCulture);
                }
            }

            return null;
        }
    }
}