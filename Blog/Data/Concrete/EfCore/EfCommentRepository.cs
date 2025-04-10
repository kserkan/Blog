using Blog.Data.Abstract;
using Blog.Entity;

namespace Blog.Data.Concrete.EfCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private BlogAppContext _context;
        public EfCommentRepository(BlogAppContext context)
        {
            _context = context;
        }
        public IQueryable<Comment> Comments => _context.Comments;

        public void CreateComment(Comment Comment)
        {
            _context.Comments.Add(Comment);
            _context.SaveChanges();
        }

        public void DeleteComment(int id)
        {
            var entity = _context.Comments.FirstOrDefault(c => c.CommentId == id);
            if (entity != null)
            {
                _context.Comments.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void EditComment(Comment comment)
        {
            var entity = _context.Comments.FirstOrDefault(c => c.CommentId == comment.CommentId);
            if (entity != null)
            {
                entity.Text = comment.Text;
                entity.PublishedOn = comment.PublishedOn;
                _context.SaveChanges();
            }
        }
    }
}
