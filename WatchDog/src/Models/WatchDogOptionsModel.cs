namespace WatchDog.src.Models
{
    public class WatchDogOptionsModel
    {
        public bool UseAuth { get; set; }
        public string WatchPageUsername { get; set; }
        public string WatchPagePassword { get; set; }
        public string Blacklist { get; set; }
    }
}
