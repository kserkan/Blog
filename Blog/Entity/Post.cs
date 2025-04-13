
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

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();

        public int CategoryId { get; set; }   
        public Category Category { get; set; } = null!;

    }
}
