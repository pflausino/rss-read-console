using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Poc.ReadRSS
{
    public class Entity
    {
        private readonly string _baseUrl = "https://noticias.adventistas.org/";

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("language")]
        public string Language { get; set; }
        [BsonElement("abbreviation")]
        public string Abbreviation { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }

        public string GetUrl()
        {

            var url = $"{_baseUrl}{Language}/{Abbreviation}/feed";
            return url;

        }
    }
}