using System;
using System.Collections.Generic;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class ActivityContext: IContext
    {
        public IMongoDatabase Database { get; }
        public ActivityContext(IMongoDatabase database)
        {
            Database = database;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            Dictionary<Type, string> types = new Dictionary<Type, string>
            {
                {typeof(UserActivity), "activies"}
            };
            
            return Database.GetCollection<T>(types[typeof(T)]);
        }
    }
}