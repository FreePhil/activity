using System;
using System.Collections;
using System.Collections.Generic;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class ActivityContext: IContext
    {
        public IMongoDatabase Database { get; }
        public IDictionary<Type, string> Mapper { get; }
        public ActivityContext(IMongoDatabase database, IDictionary<Type, string> mapper)
        {
            Database = database;
            Mapper = mapper;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return Database.GetCollection<T>(Mapper[typeof(T)]);
        }
    }
}