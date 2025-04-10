using Blog.Data.Abstract;
using Blog.Entity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Concrete.EfCore
{
    public class EfPostRepository : IPostRepository
    {
        private readonly BlogAppContext _context;

        public EfPostRepository(BlogAppContext context)
        {
            _context = context;
        }

        public IQueryable<Post> Posts => _context.Posts
                                                 .Include(p => p.Category)
                                                 .Include(p => p.Tags)
                                                 .Include(p => p.Comments);

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void EditPost(Post post)
        {
            var entity = _context.Posts.FirstOrDefault(i => i.PostId == post.PostId);

            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.Content = post.Content;
                entity.Url = post.Url;
                entity.IsActive = post.IsActive;
                entity.Image = post.Image;
                entity.CategoryId = post.CategoryId;
                entity.PublishedOn = post.PublishedOn;

                _context.SaveChanges();
            }
        }

        public void DeletePost(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.PostId == id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
        }
    }
}
