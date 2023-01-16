using MongoDB.Bson.Serialization.Attributes;

namespace WatchDog.src.Models
{
    internal class Sequence
    {
        [BsonId]
        public string _Id { get; set; }

        public int Value { get; set; }
    }
}
