using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ActivityService.Models
{
    public class SimpleUser: IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; } 
        
        [BsonIgnore]
        public DateTime CreatedAt
        {
            get { return ObjectId.Parse(Id).CreationTime.ToLocalTime();  }
        }
    }
}