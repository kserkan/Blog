using Blog.Entity.Blog.Entity;

namespace Blog.Data.Abstract
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
    }

}
