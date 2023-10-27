using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AspNetCoreIdentityExamples.Models
{
    public class Post
    {
        public Post()
        {
            Blogs = new List<Blog>();
        }
        public int PostId { get; set; }

        public string PostTitle { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}
