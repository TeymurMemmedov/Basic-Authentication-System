using System.Collections.Generic;

namespace AspNetCoreIdentityExamples.Models
{
    public class Blog
    {
        public Blog() {
            Posts = new List<Post>();
        }
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; }
       
    }
}
