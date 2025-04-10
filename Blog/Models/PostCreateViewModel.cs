using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class PostCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public int CategoryId { get; set; }  // Yeni kategori seçimi

        public List<SelectListItem>? Categories { get; set; }  // Dropdown için kategori listesi
        public IFormFile? ImageFile { get; set; }  // ✅ Yeni eklendi
        public string? Tags { get; set; } // örn: "asp.net, c#, web"

    }
}
