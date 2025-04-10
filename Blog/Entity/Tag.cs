using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
    public enum TagColors
    {
        primary, danger, warning, success, secondary, info
    }

    public class Tag
    {
        public int TagId { get; set; }

        [Required, StringLength(50)]
        public string? Text { get; set; }

        public string? Url { get; set; }

        public TagColors? Color { get; set; }

        // Post ilişkisi (Many-to-Many)
        public List<Post> Posts { get; set; } = new();
    }
}
