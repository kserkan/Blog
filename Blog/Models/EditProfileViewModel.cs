using Blog.Entity;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{

    public class EditProfileViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        public string? Image { get; set; }
        public List<Post> Posts { get; set; } = new();
    }

}
