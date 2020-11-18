using System;
using System.ComponentModel.DataAnnotations;

namespace IBAR.TradeModel.Data.Entities
{
    public interface IAuditable
    {
        [Timestamp] 
        byte[] Timestamp { get; set; }

        DateTime CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }
    }
}
