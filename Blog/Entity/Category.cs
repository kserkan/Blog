using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
    // Entity/Category.cs
    namespace Blog.Entity
    {
        public class Category
        {
            [Required]
            public int CategoryId { get; set; }

            [Required, StringLength(100)]
            public string Name { get; set; }

            public string? Url { get; set; }

            // İlişki: Bir kategoride birden fazla yazı olabilir
            public List<Post> Posts { get; set; } = new();
        }
    }

}
