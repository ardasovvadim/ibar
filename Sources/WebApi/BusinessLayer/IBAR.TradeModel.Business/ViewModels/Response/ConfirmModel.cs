using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBAR.ServiceLayer.Common;

namespace IBAR.TradeModel.Business.ViewModels.Response
{
    public class ConfirmModel : LoginModel
    {
        [Required(ErrorMessage = "VerificationCode is required.")]
        [RegularExpression(Patterns.PhoneCodePattern, ErrorMessage = "Phone code has invalid form.")]
        public int VerificationCode { get; set; }
    }
}
