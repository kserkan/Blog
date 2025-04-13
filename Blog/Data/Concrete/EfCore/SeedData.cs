using Blog.Data.Concrete.EfCore;
using Blog.Entity;
using Blog.Entity.Blog.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Data
{
    public static class SeedData
    {
        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<BlogAppContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Web Programlama", Url = "web-programlama" },
                    new Category { Name = "Mobil Uygulamalar", Url = "mobil-uygulamalar" },
                    new Category { Name = "Veritabanı Yönetimi", Url = "veritabani-yonetimi" },
                    new Category { Name = "Yazılım Geliştirme", Url = "yazilim-gelistirme" },
                    new Category { Name = "Siber Güvenlik", Url = "siber-guvenlik" },
                    new Category { Name = "Yapay Zeka", Url = "yapay-zeka" },
                    new Category { Name = "Genel", Url = "Genel" }
                );
            }

            if (!context.Users.Any())
            {
                var admin = new User
                {
                    UserName = "admin",
                    Name = "Admin Kullanıcı",
                    Email = "admin@blog.com",
                    Password = Hash("123456"),
                    Image = "p1.jpg",
                    IsAdmin = true
                };

                var user = new User
                {
                    UserName = "kullanici",
                    Name = "Deneme Kullanıcı",
                    Email = "user@blog.com",
                    Password = Hash("123456"),
                    Image = "p2.jpg",
                    IsAdmin = false
                };

                context.Users.AddRange(admin, user);
                context.SaveChanges();
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(
                    new Tag { Text = "aspnet", Url = "aspnet" },
                    new Tag { Text = "flutter", Url = "flutter" },
                    new Tag { Text = "web", Url = "web" },
                    new Tag { Text = "mobil", Url = "mobil" }
                );
                context.SaveChanges();
            }

            if (!context.Posts.Any())
            {
                var admin = context.Users.First(u => u.IsAdmin);
                var cat1 = context.Categories.First();
                var cat2 = context.Categories.Skip(1).First();
                var tags = context.Tags.ToList();

                var post1 = new Post
                {
                    Title = "ASP.NET Core ile Web Uygulaması",
                    Content = "Bu yazıda ASP.NET Core ile nasıl web uygulaması geliştirilir anlatacağız.",
                    Description = "ASP.NET Core giriş seviyesi bir makale",
                    Url = "aspnet-core-web-uygulamasi",
                    Image = "1.jpg",
                    PublishedOn = DateTime.Now.AddDays(-2),
                    IsActive = true,
                    CategoryId = cat1.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[0], tags[2] }
                };

                var post2 = new Post
                {
                    Title = "Flutter ile Mobil Geliştirme",
                    Content = "Flutter kullanarak iOS ve Android uygulamaları geliştirme adımlarını göreceğiz.",
                    Description = "Flutter başlangıç rehberi",
                    Url = "flutter-mobil-gelistirme",
                    Image = "2.jpg",
                    PublishedOn = DateTime.Now.AddDays(-1),
                    IsActive = true,
                    CategoryId = cat2.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[1], tags[3] }
                };

                var post3 = new Post
                {
                    Title = "Veritabanı Yönetimi",
                    Content = "Veritabanı yönetimi ile ilgili temel bilgiler ve SQL komutları.",
                    Description = "Veritabanı yönetimi hakkında bilgi",
                    Url = "veritabani-yonetimi",
                    Image = "1.jpg",
                    PublishedOn = DateTime.Now.AddDays(-3),
                    IsActive = true,
                    CategoryId = cat1.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[0], tags[2] }
                };

                var post4 = new Post
                {
                    Title = "Siber Güvenlik Temelleri",
                    Content = "Siber güvenlik nedir? Temel kavramlar ve önlemler.",
                    Description = "Siber güvenlik hakkında bilgi",
                    Url = "siber-guvenlik-temelleri",
                    Image = "2.jpg",
                    PublishedOn = DateTime.Now.AddDays(-4),
                    IsActive = true,
                    CategoryId = cat2.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[1], tags[3] }
                };
                var post5 = new Post
                {
                    Title = "Yapay Zeka ve Makine Öğrenimi",
                    Content = "Yapay zeka ve makine öğrenimi nedir? Temel kavramlar.",
                    Description = "Yapay zeka hakkında bilgi",
                    Url = "yapay-zeka-makine-ogrenimi",
                    Image = "1.jpg",
                    PublishedOn = DateTime.Now.AddDays(-5),
                    IsActive = true,
                    CategoryId = cat1.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[0], tags[2] }
                }; 
                var post6 = new Post
                {
                    Title = "Yazılım Geliştirme Süreçleri",
                    Content = "Yazılım geliştirme süreçleri ve metodolojileri.",
                    Description = "Yazılım geliştirme süreçleri hakkında bilgi",
                    Url = "yazilim-gelistirme-surecleri",
                    Image = "2.jpg",
                    PublishedOn = DateTime.Now.AddDays(-6),
                    IsActive = true,
                    CategoryId = cat2.CategoryId,
                    UserId = admin.UserId,
                    Tags = new List<Tag> { tags[1], tags[3] }
                };

                context.Posts.AddRange(post1, post2,post3,post4,post5,post6);
                context.SaveChanges();

                context.Comments.AddRange(
                    new Comment { PostId = post1.PostId, Text = "Harika bir içerik!", PublishedOn = DateTime.Now, UserId = admin.UserId },
                    new Comment { PostId = post2.PostId, Text = "Flutter örneği çok açıklayıcıydı.", PublishedOn = DateTime.Now, UserId = admin.UserId },
                    new Comment { PostId = post3.PostId, Text = "Veritabanı yönetimi ile ilgili daha fazla bilgi bekliyorum.", PublishedOn = DateTime.Now, UserId = admin.UserId },
                    new Comment { PostId = post4.PostId, Text = "Siber güvenlik konusunu çok iyi ele almışsınız.", PublishedOn = DateTime.Now, UserId = admin.UserId },
                    new Comment { PostId = post5.PostId, Text = "Yapay zeka ile ilgili daha fazla örnek ekleyebilirsiniz.", PublishedOn = DateTime.Now, UserId = admin.UserId },
                    new Comment { PostId = post6.PostId, Text = "Yazılım geliştirme süreçleri hakkında daha fazla bilgi bekliyorum.", PublishedOn = DateTime.Now, UserId = admin.UserId }
                );

                context.SaveChanges();
            }

            if (!context.Notifications.Any())
            {
                var admin = context.Users.First(u => u.IsAdmin);
                var user = context.Users.First(u => !u.IsAdmin);

                context.Notifications.AddRange(
                    new Notification { UserId = user.UserId, Message = "Hoş geldiniz! Profilinizi güncelleyebilirsiniz.", IsRead = false },
                    new Notification { UserId = admin.UserId, Message = "Yeni bir yazı eklendi: ASP.NET Core", IsRead = false }
                );

                context.SaveChanges();
            }
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
