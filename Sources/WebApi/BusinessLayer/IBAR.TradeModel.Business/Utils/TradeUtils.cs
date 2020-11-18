using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Business.Common.Extensions;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Business.Utils
{
    public class ExtractedAccountData
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string AccountAlias { get; set; }
        public decimal Number { get; set; }
    }
    
    public static class TradeUtils
    {
        public static string ResolveMasterAccountName(MasterAccount acc)
        {
            return string.IsNullOrEmpty(acc.AccountAlias)
                ? acc.AccountName
                : acc.AccountAlias;
        }
        
        public static IEnumerable<string> GetPeriodLabels(IEnumerable<Period> periods)
        {
            return periods.Select(p =>
            {
                if (p.FromDate.Date == p.ToDate.Date)
                {
                    return p.FromDate.ToStandardAppDateFormatString();
                }

                return p.FromDate.ToStandardAppDateFormatString() + " / " +
                       p.ToDate.ToStandardAppDateFormatString();
            });
        }
        
        public static void ProcessExtractedData(List<ExtractedAccountData> extractedData,
            Dictionary<long, ChartDataVm.DataSet> data)
        {
            var forFillZero = data.Keys.ToList();

            extractedData.ForEach(account =>
            {
                if (!data.ContainsKey(account.Id))
                {
                    data[account.Id] = new ChartDataVm.DataSet
                    {
                        Label = string.IsNullOrEmpty(account.AccountAlias)
                            ? account.AccountName
                            : account.AccountAlias
                    };
                }

                data[account.Id].Data.Add(account.Number);

                forFillZero.Remove(account.Id);
            });

            forFillZero.ForEach(accountId => { data[accountId].Data.Add(0); });
        }
        
        public static bool IsSortingByTradeAccounts(long[] idTradeAccounts)
        {
            return idTradeAccounts != null && idTradeAccounts.Length > 0;
        }

        public static bool IsSortingByMasterAccounts(long[] idMasterAccounts)
        {
            return idMasterAccounts != null && idMasterAccounts.Length >= 1 && idMasterAccounts[0] != -1;
        }
        
        
    }
}