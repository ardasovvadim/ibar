using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response.Dashboard
{
    public class ReturnDataStandart
    {
        public class TotalIncomeData
        {
            public decimal[] Data { get; set; }

            public string Label { get; set; }
        }

        public long Id { get; set; }
        public long Total { get; set; }
        public string Expression { get; set; }

        public string[] Labels { get; set; }

        public TotalIncomeData[] Data { get; set; }

        public static ReturnDataStandart Default
        {
            get
            {
                return new ReturnDataStandart()
                {
                    Labels = new string[] { },
                    Data = new TotalIncomeData[] { }
                };
            }
        }
    }
}
