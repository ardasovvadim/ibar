using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class TotalAccountListModel
    {
        public TotalAccountListEnum Enum { get; set; }
        public List<KeyValuePair<string, long>> AccountTotalList { get; set; }
    }
}
