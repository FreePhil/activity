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

        public IMongoCollection<UserActivity> Activities {
            get { return Database.GetCollection<UserActivity>("activities"); } 
        }
    }
}