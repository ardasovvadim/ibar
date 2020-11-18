using IBAR.ServiceLayer.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class MasterAccountCreateEditModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(Patterns.AccountName, ErrorMessage = "Account Name can contain the following characters: A-Z and numbers")]
        public string AccountName { get; set; }

        public string AccountAlias { get; set; }
    }
}
