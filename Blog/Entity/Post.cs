
using Blog.Entity.Blog.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Blog.Entity
{
    public class Post
    {
        public int PostId { get; set; }

        [Required, StringLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public string? Url { get; set; }
        public string? Image { get; set; }

        public DateTime PublishedOn { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public int ViewCount { get; set; } = 0;

        // Kullanıcı ilişkisi
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Etiket ilişkisi
        public List<Tag> Tags { get; set; } = new();

        // Yorum ilişkisi
        public List<Comment> Comments { get; set; } = new();

        // Entity/Post.cs içinde:
        public int CategoryId { get; set; }     // Foreign Key
        public Category Category { get; set; } = null!;

    }
}
