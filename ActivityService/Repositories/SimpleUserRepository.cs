using System;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class SimpleUserRepository: ServiceRepository<SimpleUser>, ISimpleUserRepository
    {
        public SimpleUserRepository(IContext context) : base(context)
        {
        }

        public void CreateIndex()
        {
            var indexBuilder = Builders<SimpleUser>.IndexKeys;
            var indexOption = new CreateIndexOptions {Unique = true};
            var indexModel = new CreateIndexModel<SimpleUser>(indexBuilder.Ascending(u => u.Name), indexOption);
            
            Context.GetCollection<SimpleUser>().Indexes.CreateOne(indexModel);
        }

        public async Task<SimpleUser> GetByUserNameAsync(string userName)
        {
            var user = await Context.GetCollection<SimpleUser>().Find(u => u.Name == userName).SingleAsync();
            return user;
        }

        public async Task<SimpleUser> AddAsync(string userName)
        {
            SimpleUser user = new SimpleUser {Name = userName};
            await Context.GetCollection<SimpleUser>().InsertOneAsync(user);

            return user;
        }
    }
}