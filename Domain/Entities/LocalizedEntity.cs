namespace Domain.Entities
{
    public abstract class LocalizedEntity : Entity
    {
        public Language Language { get; set; }
    }
}