using System.ComponentModel.DataAnnotations;
using IBAR.ServiceLayer.Common;

namespace IBAR.TradeModel.Business.ViewModels.Request
{
    public class ChangePasswordEditModel
    {
        public long Id { get; set; }
        public string OldPassword { get; set; }
        [RegularExpression(Patterns.PasswordPattern, ErrorMessage = "Invalid password format.")]
        public string NewPassword { get; set; }
    }
}