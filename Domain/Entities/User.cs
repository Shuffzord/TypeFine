namespace Domain.Entities
{
    public class User : LocalizedEntity
    {
        public virtual string DeviceId { get; set; }
        public virtual string AccountId { get; set; }
    }
}