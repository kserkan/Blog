using Blog.Entity;

namespace Blog.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }
        void CreateUser(User User);
        void UpdateUser(User user);

    }
}
