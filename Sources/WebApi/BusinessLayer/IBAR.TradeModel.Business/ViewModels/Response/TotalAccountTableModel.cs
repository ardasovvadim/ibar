using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class TotalAccountTableModel
    {
        public string RowName { get; set; }
        public decimal LastDay { get; set; }
        public decimal MTD { get; set; }
        public decimal LastMonth { get; set; }
        public decimal AvgDailyMonth { get; set; }
        public decimal AvgDailyYear { get; set; }
    }
}
