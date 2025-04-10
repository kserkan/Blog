// Data/Concrete/EfCore/EfCoreCategoryRepository.cs
using Blog.Data.Abstract;
using Blog.Entity;
using Blog.Entity.Blog.Entity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Concrete.EfCore
{
    public class EfCoreCategoryRepository : ICategoryRepository
    {
        private readonly BlogAppContext _context;
        public EfCoreCategoryRepository(BlogAppContext context)
        {
            _context = context;
        }

        public IQueryable<Category> Categories => _context.Categories;
    }
}
