using IBAR.ServiceLayer.Common;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IBAR.Api.Models
{
    public class UserCreateEditModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(Patterns.NamePattern, ErrorMessage = "First Name can contain the following characters: A-Z,a-z.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(Patterns.NamePattern, ErrorMessage = "Last Name can contain the following characters: A-Z,a-z.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(Patterns.EmailPattern, ErrorMessage = "Email is invalid.")]
        public string Email { get; set; }

        [Display(Name = "phone")]
        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(Patterns.PhonePattern, ErrorMessage = "Phone is invalid.")]
        public string Phone { get; set; }
    }
}