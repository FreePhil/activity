using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ActivityService.Models
{
    public class UserActivity
    {
        public ObjectId Id { get; set; }
        
        public string UserId { get; set; }
        
        public string Option { get; set; }
        public string Payload { get; set; }
        public string Status { get; set; }
        
        public DateTime UpdatedAt { get; set; } 
        
        [BsonIgnore]
        public DateTime CreatedAt
        {
            get { return Id.CreationTime;  }
        }
    }
}