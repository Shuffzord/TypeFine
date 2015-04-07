using System;

namespace Domain.Entities
{
    public class Notifications : Entity
    {
        public virtual string Value { get; set; }
        public virtual DateTime DateSent { get; set; }
        public virtual int NotificationType { get; set; }
    }
}