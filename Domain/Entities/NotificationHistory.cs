namespace Domain.Entities
{
    public class NotificationHistory : LocalizedEntity
    {
        public virtual User User { get; set; }
        public virtual Notifications Notifications { get; set; }
    }
}