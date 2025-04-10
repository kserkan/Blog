using Blog.Data.Abstract;
using Blog.Entity;

namespace Blog.Data.Concrete.EfCore
{
    public class EfNotificationRepository : INotificationRepository
    {
        private readonly BlogAppContext _context;
        public EfNotificationRepository(BlogAppContext context)
        {
            _context = context;
        }

        public IQueryable<Notification> Notifications => _context.Notifications;

        public void Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public void MarkAsRead(int id)
        {
            var notif = _context.Notifications.Find(id);
            if (notif != null)
            {
                notif.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}
