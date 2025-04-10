using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Blog.Entity
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string? UserName { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? Image { get; set; }
        public bool IsAdmin { get; set; } = false;


        // İlişkiler
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}
