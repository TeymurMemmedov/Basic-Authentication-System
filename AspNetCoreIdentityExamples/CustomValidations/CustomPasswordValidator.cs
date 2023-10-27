using AspNetCoreIdentityExamples.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentityExamples.CustomValidations
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {

        
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();


            if (password.Length > 16)
                errors.Add(
                    new IdentityError
                    {
                        Code = "PasswordLength",
                        Description = "Please enter the password that shorter than 16 characters"
                    });
            if (password.ToLower().Contains(user.UserName))
                errors.Add(
                    new IdentityError
                    {
                        Code = "PasswordContainsUsername",
                        Description = "Please dont include your username to password"
                    });
            if (!errors.Any())
                return Task.FromResult(IdentityResult.Success);
            else
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}
