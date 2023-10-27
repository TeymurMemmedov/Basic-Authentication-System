using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityExamples.ViewModels
{
    public class AttachmentViewModel
    {
        [Required]
        [Display(Name = "Select Files")]
        public IFormFileCollection Attachments { get; set; }
    }
}
