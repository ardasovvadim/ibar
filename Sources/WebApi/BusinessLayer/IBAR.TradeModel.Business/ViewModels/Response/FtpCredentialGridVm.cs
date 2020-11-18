using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBAR.ServiceLayer.ViewModels;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class FtpCredentialGridVm
    {
        [Required]
        public long Id;
        [Required]
        public string FtpName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserPassword { get; set; }
        [Required]
        public string Url { get; set; }
        public IEnumerable<MasterAccountVm> MasterAccounts { get; set; }
    }
}