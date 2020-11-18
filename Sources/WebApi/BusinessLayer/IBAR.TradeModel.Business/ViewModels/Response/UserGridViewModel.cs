using System.Collections.Generic;

namespace IBAR.ServiceLayer.ViewModels
{
    public class UserGridViewModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        
        public bool IsWaitingConfirmation { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
