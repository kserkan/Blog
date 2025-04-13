using Blog.Data;
using Blog.Data.Abstract;
using Blog.Data.Concrete;
using Blog.Data.Concrete.EfCore;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogAppContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var apiKey = builder.Configuration["OpenAI:ApiKey"];
builder.Services.AddScoped<OpenAIService>();

builder.Services.AddHttpClient();

// Repository DI
builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>(); // 👈 Eksikti, eklendi
builder.Services.AddScoped<INotificationRepository, EfNotificationRepository>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
        options.AccessDeniedPath = "/Users/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        
        options.ClaimsIssuer = ClaimTypes.Role;

    });

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Test verilerini doldur (varsa)
SeedData.TestVerileriniDoldur(app);


// Routing
app.MapControllerRoute(
    name: "post_details",
    pattern: "blogs/{url}",
    defaults: new { controller = "Posts", action = "Details" });


app.MapControllerRoute(
    name: "blogs_query_string",
    pattern: "blogs",
    defaults: new { controller = "Posts", action = "Index" });


app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

app.Run();
//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