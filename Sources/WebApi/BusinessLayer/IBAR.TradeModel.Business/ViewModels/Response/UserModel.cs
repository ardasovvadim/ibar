using System.Collections.Generic;

namespace IBAR.ServiceLayer.ViewModels
{
    public class UserModel : EntityModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string AccessToken { get; set; }
        public int VerificationCode { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}