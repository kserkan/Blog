using Blog.Data.Abstract;
using Blog.Entity;

namespace Blog.Data.Concrete.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private BlogAppContext _context;
        public EfTagRepository(BlogAppContext context)
        {
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags;

        public void CreateTag(Tag Tag)
        {
            _context.Tags.Add(Tag);
            _context.SaveChanges();
        }
    }
}
