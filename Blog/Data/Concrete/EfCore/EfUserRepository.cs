using Blog.Data.Abstract;
using Blog.Entity;

namespace Blog.Data.Concrete.EfCore
{
    public class EfUserRepository : IUserRepository
    {
        private BlogAppContext _context;
        public EfUserRepository(BlogAppContext context)
        {
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;

        public void CreateUser(User User)
        {
            _context.Users.Add(User);
            _context.SaveChanges();
        }


        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

    }
}
