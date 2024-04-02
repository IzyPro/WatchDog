using WatchDog.src.Enums;

namespace WatchDog.src.Models
{
    public class WatchDogOptionsModel
    {
        public string WatchPageUsername { get; set; }
        public string WatchPagePassword { get; set; }
        public string Blacklist { get; set; }
        public string CorsPolicy { get; set; } = string.Empty;
        public WatchDogSerializerEnum Serializer { get; set; } = WatchDogSerializerEnum.Default;
        public string Tag { get; set; } = string.Empty; // Allows you to tag your microservices with a specific value.
        public string HeaderNameEventId { get; set; } = string.Empty; // Allows you to correlate logging events across different services that are part of the same transaction.
    }
}
