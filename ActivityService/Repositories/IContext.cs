using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public interface IContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<UserActivity> Activities { get; }
    }
}