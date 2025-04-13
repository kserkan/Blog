using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
   
    namespace Blog.Entity
    {
        public class Category
        {
            [Required]
            public int CategoryId { get; set; }

            [Required, StringLength(100)]
            public string Name { get; set; }

            public string? Url { get; set; }
    
            public List<Post> Posts { get; set; } = new();
        }
    }

}
