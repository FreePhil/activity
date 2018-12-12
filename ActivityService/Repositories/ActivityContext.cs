using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class ActivityContext: IContext
    {
        public ActivityContext(IMongoDatabase database)
        {
            Database = database;
        }

        public ActivityContext()
        {
            var client = new MongoClient("mongodb://localhost");
            Database = client.GetDatabase("testbank");
        }

        public IMongoDatabase Database { get; }
        public IMongoCollection<UserActivity> Activities {
            get { return Database.GetCollection<UserActivity>("activities"); } 
        }
    }
}