using System;

namespace Domain.Entities
{
    public class NotificationUser : Entity
    {
        public User User { get; set; }
        public string ChannelName { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}