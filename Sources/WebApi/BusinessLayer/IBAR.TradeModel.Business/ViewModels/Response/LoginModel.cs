using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBAR.ServiceLayer.Common;

namespace IBAR.TradeModel.Business.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(Patterns.EmailPattern, ErrorMessage = "Email is invalid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
