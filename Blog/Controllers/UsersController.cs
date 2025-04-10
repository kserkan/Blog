using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Data.Abstract;
using Blog.Data.Concrete.EfCore;
using Blog.Entity;
using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        public UsersController(IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        // 🔐 Şifre hashleyen yardımcı fonksiyon
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.Username || x.Email == model.Email);
                if (user == null)
                {
                    _userRepository.CreateUser(new User
                    {
                        UserName = model.Username,
                        Name = model.Name,
                        Email = model.Email,
                        Password = HashPassword(model.Password), // ✅ Hashleme burada
                        Image = "p1.jpg"
                    });
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Username ya da Email kullanımda.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password);
                var isUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == hashedPassword);

                if (isUser != null)
                {
                    var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, isUser.UserName ?? ""),
                new Claim(ClaimTypes.GivenName, isUser.Name ?? ""),
                new Claim(ClaimTypes.UserData, isUser.Image ?? "")
            };

                    if (isUser.IsAdmin) // ✅ burası önemli
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                }
            }

            return View(model);
        }


        [Authorize]
        public IActionResult Profile(string username)
        {
            var user = _userRepository.Users
                .Include(u => u.Posts)
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Image = user.Image,
                Posts = user.Posts.ToList()
            };
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var unread = _notificationRepository.Notifications
                .Where(n => n.UserId == currentUserId && !n.IsRead)
                .ToList();

            foreach (var n in unread)
            {
                _notificationRepository.MarkAsRead(n.Id);
            }

            return View("Profile", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar(IFormFile AvatarFile)
        {
            if (AvatarFile == null || AvatarFile.Length == 0)
            {
                TempData["Message"] = "Dosya seçilmedi.";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Profile", new { username = User.Identity.Name });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _userRepository.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            // Yeni dosya adı
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            // Kaydet
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await AvatarFile.CopyToAsync(stream);
            }

            // Mevcut görseli silmek istersen:
            // if (user.Image != null && user.Image != "p1.jpg")
            //     System.IO.File.Delete(Path.Combine("wwwroot/img", user.Image));

            // Kullanıcının görselini güncelle
            user.Image = fileName;
            _userRepository.UpdateUser(user); // Bu metot yoksa oluşturmalısın

            TempData["Message"] = "Profil fotoğrafınız güncellendi.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Profile", new { username = user.UserName });
        }


        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _userRepository.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _userRepository.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = HashPassword(model.NewPassword); // Şifre hash'lenmeli
            }

            _userRepository.UpdateUser(user);

            TempData["Message"] = "Profiliniz güncellendi.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Profile", new { username = user.UserName });
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Profile", new { username = User.Identity?.Name });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _userRepository.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.Password = HashPassword(model.NewPassword);
            }

            _userRepository.UpdateUser(user);

            TempData["Message"] = "Profil güncellendi.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Profile", new { username = user.UserName });
        }


        [Authorize]
        public IActionResult Notifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var notifications = _notificationRepository.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }

    }
}
