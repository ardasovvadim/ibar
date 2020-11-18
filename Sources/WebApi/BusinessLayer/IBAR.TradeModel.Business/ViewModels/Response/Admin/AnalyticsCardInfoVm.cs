using System;

namespace IBAR.TradeModel.Business.ViewModels.Response.Admin
{
    public class AnalyticsCardInfoVm
    {
        public int RegisteredFiles { get; set; }
        public int ImportedFiles { get; set; }

        public int ZohoFiles { get; set; }
        public int SentFiles { get; set; }
        
        public DateTime? LastSeenCopyJob { get; set; }
        public DateTime? LastSeenFtpJob { get; set; }
        public DateTime? LastSeenImportJob { get; set; }
        public DateTime? LastSeenZohoJob { get; set; }
        
        
    }
}