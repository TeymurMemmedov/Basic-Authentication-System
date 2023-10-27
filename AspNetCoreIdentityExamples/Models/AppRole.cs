using Microsoft.AspNetCore.Identity;
using System;

namespace AspNetCoreIdentityExamples.Models
{
    //AppRole-un Primary Key-i int olsun deyə İdentityRole classının bu növündən inherit edirik
    public class AppRole:IdentityRole<int>
    {
        public DateTime CreatedTime { get; set; }
    }
}
