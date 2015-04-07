using Domain.Enums;

namespace Domain.Entities
{
    public class RequestInfo : Entity
    {
        public string DeviceId { get; set; }
        public string AccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}