using Blog.Entity;

namespace Blog.Data.Abstract
{
    public interface ITagRepository
    {
        IQueryable<Tag> Tags { get; }
        void CreateTag(Tag Tag);
    }
}
