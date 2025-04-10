using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required, StringLength(300)]
        public string? Text { get; set; }

        public DateTime PublishedOn { get; set; } = DateTime.Now;

        // Post ilişkisi
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        // Kullanıcı ilişkisi
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
