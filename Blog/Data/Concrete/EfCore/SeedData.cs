using Blog.Data.Concrete.EfCore;
using Blog.Entity;
using Blog.Entity.Blog.Entity;
using Microsoft.EntityFrameworkCore;

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
                    new Category { Name = "Mobil Uygulamalar", Url = "mobil-uygulamalar" }
                );
            }

            if (!context.Users.Any())
            {
                context.Users.Add(
                    new User
                    {
                        UserName = "admin",
                        Name = "Admin Kullanıcı",
                        Email = "admin@blog.com",
                        Password = "123456",
                        Image = "p1.jpg"
                    });
            }

            context.SaveChanges(); // user ve category'lerin ID'lerini almak için

            if (!context.Posts.Any())
            {
                var post1 = new Post
                {
                    Title = "ASP.NET Core ile Web Uygulaması",
                    Content = "Bu yazıda ASP.NET Core ile nasıl web uygulaması geliştirilir anlatacağız.",
                    Description = "ASP.NET Core giriş seviyesi bir makale",
                    Url = "aspnet-core-web-uygulamasi",
                    Image = "1.jpg",
                    PublishedOn = DateTime.Now,
                    IsActive = true,
                    CategoryId = context.Categories.First().CategoryId,
                    UserId = context.Users.First().UserId,
                    Tags = new List<Tag>()
                };

                var post2 = new Post
                {
                    Title = "Flutter ile Mobil Geliştirme",
                    Content = "Flutter kullanarak iOS ve Android uygulamaları geliştirme adımlarını göreceğiz.",
                    Description = "Flutter başlangıç rehberi",
                    Url = "flutter-mobil-gelistirme",
                    Image = "2.jpg",
                    PublishedOn = DateTime.Now,
                    IsActive = true,
                    CategoryId = context.Categories.Skip(1).First().CategoryId,
                    UserId = context.Users.First().UserId,
                    Tags = new List<Tag>()
                };

                var tags = new[]
                {
                    new Tag { Text = "aspnet" },
                    new Tag { Text = "flutter" },
                    new Tag { Text = "web" },
                    new Tag { Text = "mobil" }
                };

                post1.Tags.Add(tags[0]);
                post1.Tags.Add(tags[2]);

                post2.Tags.Add(tags[1]);
                post2.Tags.Add(tags[3]);

                context.Posts.AddRange(post1, post2);
                context.Tags.AddRange(tags);

                context.Comments.AddRange(
                    new Comment { Post = post1, Text = "Harika bir içerik!", PublishedOn = DateTime.Now, UserId = context.Users.First().UserId },
                    new Comment { Post = post2, Text = "Flutter örneği çok açıklayıcıydı.", PublishedOn = DateTime.Now, UserId = context.Users.First().UserId }
                );

                context.SaveChanges();
            }
        }
    }
}