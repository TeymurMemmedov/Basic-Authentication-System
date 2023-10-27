using AspNetCoreIdentityExamples.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentityExamples.CustomValidations
{
    public class CustomUserValidation : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (int.TryParse(user.UserName[0].ToString(), out int _)) //Kullanıcı adının sayısal ifadeyle başlamaması kontrolü
                errors.Add(new IdentityError { Code = "UserNameNumberStartWith", Description = "Username cannot begin with a numeric expression..." });
            if (user.UserName.Length < 3 && user.UserName.Length > 25)
                errors.Add(new IdentityError { Code = "UserNameLenhth", Description = "Username must be between 3 and 15 characters long..." });
            if (user.Email.Length > 70)
                errors.Add(new IdentityError { Code = "EmailLenhth", Description = "Email cannot exceed 70 characters..." });

            if (!errors.Any())
                return Task.FromResult(IdentityResult.Success);
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}
