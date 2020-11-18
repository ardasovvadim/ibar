using System;
using System.Collections.Generic;
using IBAR.TradeModel.Business.ViewModels.Request;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class ChartDataVm
    {
        public class DataSet
        {
            public List<decimal> Data { get; set; }

            public string Label { get; set; }

            public DataSet()
            {
                Data = new List<decimal>();
            }
        }
        
        public ChartDataVm()
        {
        }
        
        public long Id { get; set; }

        public string[] Labels { get; set; }

        public DataSet[] Data { get; set; }

        public static ChartDataVm Default
        {
            get
            {
                return new ChartDataVm()
                {
                    Labels = new string[] { },
                    Data = new DataSet[] { }
                };
            }
        }
    }
}
