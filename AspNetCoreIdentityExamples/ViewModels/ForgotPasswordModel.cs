﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityExamples.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
