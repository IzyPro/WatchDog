using WatchDog.src.Enums;

namespace WatchDog.src.Models
{
    public class WatchDogOptionsModel
    {
        public string WatchPageUsername { get; set; }
        public string WatchPagePassword { get; set; }
        public string Blacklist { get; set; }
        public string CorsPolicy { get; set; } = string.Empty;
        public bool UseOutputCache { get; set; }
        public bool UseRegexForBlacklisting { get; set; }
        public WatchDogSerializerEnum Serializer { get; set; } = WatchDogSerializerEnum.Default;
    }
}
