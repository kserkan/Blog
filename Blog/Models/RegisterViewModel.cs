using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        [Required]
        [Display(Name = "Ad Soyad")]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "{0} en az {2} karakter olmalı.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string? Password { get; set; }
    }

}
