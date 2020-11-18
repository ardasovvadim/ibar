using System;

namespace IBAR.ServiceLayer.Utils
{
    public static class DateTimeExtension
    {
        public static string ToStringParam(this DateTime date)
        {
            return date.ToString("yyyy-dd-MM");
        }
    }
}