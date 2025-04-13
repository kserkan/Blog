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

        // Cevaplar
        public int? ParentCommentId { get; set; } 
        public Comment? ParentComment { get; set; }
        public ICollection<Comment>? Replies { get; set; }


        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
