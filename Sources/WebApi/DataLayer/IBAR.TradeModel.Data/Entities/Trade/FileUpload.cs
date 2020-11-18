using System;

namespace IBAR.TradeModel.Data.Entities.Trade
{
    public class FileUpload : EntityBase
    {
        public virtual ImportedFile ImportedFile { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
