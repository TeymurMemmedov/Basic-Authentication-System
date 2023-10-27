using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityExamples.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please do not enter a blank email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter an email address in the correct format.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please leave the password empty.")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a password in the appropriate format.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Remember me...
        /// </summary>
        [Display(Name = "Remember Me")]
        public bool Persistent { get; set; }
        public bool Lock { get; set; }
    }
}
