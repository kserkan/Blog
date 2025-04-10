using Blog.Entity;

namespace Blog.Data.Abstract
{

    public interface ICommentRepository
    {
        IQueryable<Comment> Comments { get; }
        void CreateComment(Comment Comment);

        void EditComment(Comment Comment);
        void DeleteComment(int id);
    }
}