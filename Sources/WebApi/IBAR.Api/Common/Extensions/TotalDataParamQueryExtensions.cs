using System;
using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Business.Data;
using IBAR.TradeModel.Business.ViewModels;

namespace IBAR.Api.Common.Extensions
{
    public static class TotalDataParamQueryExtensions
    {
        public static IEnumerable<TEnum> ToDashboardEnumTypes<TEnum>(this IEnumerable<int> dashboardTypes) where TEnum : struct
        {
            return dashboardTypes.Select(type =>
            {
                if (Enum.TryParse<TEnum>(type.ToString(), out var typeEnum))
                {
                    return typeEnum;
                }

                throw new ArgumentException();
            });
        }
    }
}