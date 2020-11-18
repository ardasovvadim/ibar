using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IBAR.ServiceLayer.ViewModels
{
    public class UserRoleModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Roles are required.")]
        public IEnumerable<string> Roles { get; set; }
    }
}
