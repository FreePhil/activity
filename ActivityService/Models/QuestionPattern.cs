using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ActivityService.Models
{
    public class QuestionPattern: IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string UserId { get; set; }
        public string SubjectName { get; set; }
        public string ProductName { get; set; }
        public string PatternName { get; set; }
        public string Payload { get; set; }
        public IList<PatternItem> Items { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; } 
        
        [BsonIgnore]
        public DateTime CreatedAt
        {
            get { return ObjectId.Parse(Id).CreationTime.ToLocalTime();  }
        }
    }
}