using System.Collections.Generic;

namespace IBAR.Api.Data
{
    public class ClientTradesModel
    {
        public string ClientId { get; set; }
        public IEnumerable<int> Enums { get; set; }
        public PeriodString Period { get; set; }
    }
}