using System.Collections.Generic;

namespace IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles
{
    public class PaginationModel<T> where T : class
    {
        public List<T> Data { get; set; }
        public long DataLength { get; set; }
    }
}