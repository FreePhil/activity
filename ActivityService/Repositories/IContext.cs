using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public interface IContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<T> GetCollection<T>();
    }
}