using System;
using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Business.Data;

namespace IBAR.TradeModel.Business.ViewModels.Response.Dashboard
{
    public class TotalDataVm<T> where T : Enum
    {
        public IList<decimal> Totals { get; set; }

        public TotalDataVm()
        {
            Totals = Enumerable.Repeat(decimal.Zero, Enum.GetNames(typeof(T)).Length).ToList();
        }

        public decimal this[int index]
        {
            get => Totals.ToList()[index];
            set => Totals[index] = value;
        }
    }
}