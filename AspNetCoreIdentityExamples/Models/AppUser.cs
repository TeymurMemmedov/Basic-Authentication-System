using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityExamples.Models
{
    //AppUser-in Primary Key-i int olsun deyə İdentityUser classının bu növündən inherit edirik
    public class AppUser: IdentityUser<int>
    {
        public string Address { get; set; }
        public string Gender { get; set; }
        
    }
}
