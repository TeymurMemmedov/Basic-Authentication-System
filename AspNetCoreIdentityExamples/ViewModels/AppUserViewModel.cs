using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityExamples.ViewModels
{
    public class AppUserViewModel
    {
        [Required(ErrorMessage = "Please leave username empty...")]
        [StringLength(15, ErrorMessage = "Please enter username between 4 and 15 characters...", MinimumLength = 4)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please do not leave the email blank...")]
        [EmailAddress(ErrorMessage = "Please enter a value in email format...")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please leave the password blank...")]
        [DataType(DataType.Password, ErrorMessage = "Please enter password following all rules...")]
        [Display(Name = "Password")]

        public string NotHashedPassword { get; set; }

        [Required(ErrorMessage = "Please leave the password blank...")]
        [DataType(DataType.Password, ErrorMessage = "Please enter password following all rules...")]
        [Display(Name = "Re-enter password")]
        [Compare(nameof(NotHashedPassword), ErrorMessage = "The passwords you entered are not the same")]
        public string ReNotHashedPassword { get; set; }


    }
}
