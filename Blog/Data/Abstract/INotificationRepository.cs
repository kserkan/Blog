using Blog.Entity;

namespace Blog.Data.Abstract
{
    public interface INotificationRepository
    {
        IQueryable<Notification> Notifications { get; }

        void Create(Notification notification);
        void MarkAsRead(int id);
    }
}
