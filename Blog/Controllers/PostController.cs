using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Blog.Data.Abstract;
using Blog.Data.Concrete.EfCore;
using Blog.Entity;
using Blog.Helpers;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Helpers;
namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRrepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly OpenAIService _openAIService;
        private readonly IUserRepository _userRepository;

        public PostsController(IPostRepository postRrepository, ICommentRepository commentRepository,
            ICategoryRepository categoryRepository,ITagRepository tagRepository, INotificationRepository notificationRepository,
            OpenAIService openAIService,IUserRepository userRepository )
        {
            _postRrepository = postRrepository;
            _commentRepository = commentRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _notificationRepository = notificationRepository;
            _openAIService = openAIService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(int? categoryId, string? search, string? tag, int page = 1)
        {
            Console.WriteLine("TAG PARAMETRESİ GELDİ: " + tag);

            int pageSize = 5; // her sayfada gösterilecek yazı sayısı

            var posts = _postRrepository.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (categoryId != null)
            {
                posts = posts.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Text == tag));
            }

            ViewBag.Categories = _categoryRepository.Categories.ToList();
            ViewBag.TagFilter = tag;

            int totalPosts = await posts.CountAsync();
            var postsOnPage = await posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PostViewModel
            {
                Posts = postsOnPage,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize)
            };

            return View(model);
        }





        public async Task<IActionResult> Details(string url)
        {
            var post = await _postRrepository.Posts
                .Include(x => x.Tags)
                .Include(x => x.Comments).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(p => p.Url == url);

            if (post == null)
            {
                return NotFound(); // post null ise hata vermek yerine kullanıcıyı bilgilendir
            }

            post.ViewCount++;
            _postRrepository.EditPost(post);

            return View(post);
        }



        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddComment(int PostId, string Text, int? ParentCommentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            //AI kontrolü
            var result = await _openAIService.AnalyzeCommentAsync(Text);
            Console.WriteLine("AI YANITI: " + result); // DEBUG


            if (result == "hata" || result.Contains("uygunsuz"))
            {
                // Admine bildirim gönder
                var adminUsers = _userRepository.Users.Where(u => u.IsAdmin).ToList();
                Console.WriteLine("Admin Kullanıcılar: " + string.Join(", ", adminUsers.Select(u => u.UserName)));
                foreach (var admin in adminUsers)
                {
                    if (admin.UserId != int.Parse(userId ?? "0"))
                    {
                        Console.WriteLine($"Bildirim gönderiliyor: {admin.UserName}");
                        _notificationRepository.Create(new Notification
                        {
                            UserId = admin.UserId,
                            Message = $"{username} adlı kullanıcı uygunsuz yorum denemesi yaptı: \"{Text}\""
                        });
                    }
                }


                return Json(new { success = false, message = "Yorum içeriği uygun değil. (AI kontrol başarısız olabilir)" });
            }




            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(userId ?? "0"),
                ParentCommentId = ParentCommentId
            };

            _commentRepository.CreateComment(entity);

            // Bildirim gönder
            var post = _postRrepository.Posts.FirstOrDefault(p => p.PostId == PostId);
            if (post != null && post.UserId != entity.UserId)
            {
                _notificationRepository.Create(new Notification
                {
                    UserId = post.UserId,
                    Message = $"{username} adlı kullanıcı \"{post.Title}\" adlı yazınıza yorum yaptı."
                });
            }

            return Json(new
            {
                success = true,
                username,
                Text,
                entity.PublishedOn,
                avatar,
                commentId = entity.CommentId,
                userId = entity.UserId,
                parentCommentId = ParentCommentId
            });
        }






        [Authorize]
        public IActionResult Create()
        {
            var categories = _categoryRepository.Categories.ToList();

            var model = new PostCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = "1.jpg";

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }
                }

                var tagList = model.Tags?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLower())
                    .Distinct()
                    .ToList();

                var tags = new List<Tag>();
                foreach (var tagText in tagList ?? new List<string>())
                {
                    var tagUrl = tagText.Replace(" ", "-");
                    var existingTag = _postRrepository.Posts.SelectMany(p => p.Tags)
                                           .FirstOrDefault(t => t.Text == tagText);

                    if (existingTag != null)
                        tags.Add(existingTag);
                    else
                        tags.Add(new Tag { Text = tagText, Url = tagUrl, Color = TagColors.primary });
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    Url = model.Title.ToSeoUrl(),

                    CategoryId = model.CategoryId,
                    UserId = userId,
                    PublishedOn = DateTime.Now,
                    Image = fileName,
                    IsActive = User.IsInRole("admin"),
                    Tags = tags
                };

                _postRrepository.CreatePost(post);

                return Json(new { success = true, message = "Yazı başarıyla eklendi!" });
            }

            return Json(new { success = false, message = "Lütfen eksik alanları doldurun." });
        }




        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);
            var isAdmin = User.IsInRole("admin");

            var posts = _postRrepository.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable();

            if (!isAdmin)
            {
                posts = posts.Where(p => p.UserId == userId);
            }

            return View(await posts.OrderByDescending(p => p.PublishedOn).ToListAsync());
        }


        [Authorize]
        public IActionResult Edit(int id)
        {
            var post = _postRrepository.Posts.FirstOrDefault(p => p.PostId == id);
            if (post == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            // Admin HER yazıyı görebilmeli
            if (!isAdmin && post.UserId != userId)
                return Forbid(); // 403 daha doğru

            // Kategori verileri edit formu için gerekiyor
            ViewBag.Categories = _categoryRepository.Categories.ToList();
            ViewBag.AllTags = _tagRepository.Tags.Select(t => t.Text).Distinct().ToList();


            return View(post);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Edit(Post model)
        {
            var existingPost = _postRrepository.Posts.FirstOrDefault(p => p.PostId == model.PostId);
            if (existingPost == null)
                return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(); // kullanıcı girişi yoksa
            ViewBag.Categories = _categoryRepository.Categories.ToList();
            var userId = int.Parse(userIdStr);
            var isAdmin = User.IsInRole("admin");

            // Admin HER yazıyı düzenleyebilir
            if (!isAdmin && existingPost.UserId != userId)
                return Forbid(); // 403 - giriş var ama yetki yok

            // Güncelleme
            existingPost.Title = model.Title;
            existingPost.Content = model.Content;
            existingPost.CategoryId = model.CategoryId;
            existingPost.Url = model.Title.ToSeoUrl();


            _postRrepository.EditPost(existingPost);

            TempData["Message"] = "Yazı güncellendi.";
            TempData["MessageType"] = "success";

            return RedirectToAction("List");
        }



        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken] // Bu endpoint'e sadece JavaScript ile JSON POST (fetch/ajax) gönderildiği için CSRF token kontrolü devre dışı bırakıldı.
        public JsonResult DeletePost([FromBody] int id)
        {
            var post = _postRrepository.Posts.FirstOrDefault(p => p.PostId == id);
            if (post == null)
                return Json(new { success = false, message = "Yazı bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && post.UserId != userId)
                return Json(new { success = false, message = "Yetkiniz yok." });

            (_postRrepository as EfPostRepository)?.DeletePost(post.PostId);
            // 🔔 Bildirim gönder
            if (isAdmin && post.UserId != userId)  // sadece admin başka birinin yazısını siliyorsa
            {
                _notificationRepository.Create(new Notification
                {
                    UserId = post.UserId,
                    Message = $"\"{post.Title}\" adlı yazınız admin tarafından silindi."
                });
            }
            return Json(new { success = true, message = "Yazı başarıyla silindi." });
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Approve([FromBody] JsonElement data)
        {
            int id = data.GetProperty("id").GetInt32(); 

            var post = _postRrepository.Posts.FirstOrDefault(p => p.PostId == id);
            if (post == null)
                return Json(new { success = false, message = "Yazı bulunamadı." });

            post.IsActive = true;
            _postRrepository.EditPost(post);
            //Bildirim gönder
            _notificationRepository.Create(new Notification
            {
                UserId = post.UserId,
                Message = $"\"{post.Title}\" adlı yazınız admin tarafından yayına alındı."
            });
            return Json(new { success = true, message = "Yazı yayına alındı." });
        }



        [Authorize]
        public IActionResult Comments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            var comments = _commentRepository.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .Where(c => isAdmin || c.UserId == userId) // admin tümünü görür, yazar sadece kendi yorumlarını
                .OrderByDescending(c => c.PublishedOn)
                .ToList();

            return View("Comments", comments);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            var comment = _commentRepository.Comments.FirstOrDefault(c => c.CommentId == id);
            if (comment == null)
                return Json(new { success = false, message = "Yorum bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            // Sadece yorum sahibi veya admin silebilir
            if (!isAdmin && comment.UserId != userId)
                return Json(new { success = false, message = "Bu yorumu silmeye yetkiniz yok." });

            _commentRepository.DeleteComment(id);

            return Json(new { success = true, message = "Yorum silindi." });
        }



        [Authorize]
        public IActionResult EditComment(int id)
        {
            var comment = _commentRepository.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefault(c => c.CommentId == id);

            if (comment == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            // Sadece yorum sahibi veya admin erişebilir
            if (!isAdmin && comment.UserId != userId)
                return Forbid();

            return View("~/Views/Comments/Edit.cshtml", comment);

        }

        [HttpPost]
        [Authorize]
        public IActionResult EditComment(int CommentId, string Text)
        {
            var comment = _commentRepository.Comments.FirstOrDefault(c => c.CommentId == CommentId);
            if (comment == null)
                return Json(new { success = false, message = "Yorum bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && comment.UserId != userId)
                return Json(new { success = false, message = "Yetkiniz yok." });

            comment.Text = Text;
            comment.PublishedOn = DateTime.Now;

            _commentRepository.EditComment(comment);

            return Json(new { success = true, message = "Yorum güncellendi." });
        }



    }
}
