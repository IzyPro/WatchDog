using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    internal class Sequence
    {
        [BsonId]
        public string _Id { get; set; }

        public int Value { get; set; }
    }
}
