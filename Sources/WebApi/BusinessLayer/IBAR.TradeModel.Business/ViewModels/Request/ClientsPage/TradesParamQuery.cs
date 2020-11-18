using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Request.ClientsPage
{
    public class TradesParamQuery
    {
        public string DateString { get; set; }
        public string AccountName { get; set; }
        public int PageLength { get; set; }
        public int PageIndex { get; set; }
    }
}
