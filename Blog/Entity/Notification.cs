namespace Blog.Entity
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        
        public User User { get; set; }
    }

}
