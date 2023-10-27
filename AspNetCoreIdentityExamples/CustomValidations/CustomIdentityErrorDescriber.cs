using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityExamples.CustomValidations
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName) => 
            new IdentityError { Code = "DuplicateUserName", Description = $"\"{userName}\" already used" };
        public override IdentityError InvalidUserName(string userName) =>
            new IdentityError { Code = "InvalidUserName", Description = "Please enter a valid username." };
        public override IdentityError DuplicateEmail(string email) 
            => new IdentityError { Code = "DuplicateEmail", Description = $"\"{email}\" used by another user." };
        public override IdentityError InvalidEmail(string email) 
            => new IdentityError { Code = "InvalidEmail", Description = "Invalid email." };

        public override IdentityError PasswordTooShort(int length) =>
            new IdentityError { Code = "PasswordTooShort", Description = $"The password must be at least {length} long " };
    }
}
