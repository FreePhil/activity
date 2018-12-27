using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ActivityService.Models
{
    public class UserActivity: IServiceEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string UserId { get; set; }
        
        public string Option { get; set; }
        public string Payload { get; set; }
        public string Status { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; } 
        
        [BsonIgnore]
        public DateTime CreatedAt
        {
            get { return ObjectId.Parse(Id).CreationTime.ToLocalTime();  }
        }
    }
}