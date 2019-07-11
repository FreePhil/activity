using System;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class HibernationRepository: ServiceRepository<Hibernation>, IHibernationRepository
    {
        public HibernationRepository(IContext context) : base(context)
        {
        }

        public void CreateIndex()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Hibernation> GetAsync(string userId, string subjectName, string productName)
        {
            var awaking = await Context.GetCollection<Hibernation>()
                .Find(u => u.UserId == userId && u.SubjectName == subjectName && u.ProductName == productName)
                .FirstOrDefaultAsync();
            
            return awaking;
        }
        
        public async Task UpdateAsync(string userId, string subjectName, string productName, StagePayload stage)
        {
            var awaking = await GetAsync(userId, subjectName, productName);
            if (awaking == null)
            {
                // create for the first time
                //
                await AddAsync(new Hibernation()
                {
                    UserId = userId,
                    SubjectName = subjectName,
                    ProductName = productName,
                    Stage = new StagePayload()
                    {
                        Name = stage.Name,
                        Payload = stage.Payload,
                        History = null
                    }
                });
            }
            else
            {
                // update stage 
                UpdateDefinition<Hibernation> updateQuery;
                if (awaking.Stage.Name == stage.Name)
                {
                    // replace stage
                    //
                    updateQuery = Builders<Hibernation>.Update
                        .Set(h => h.Stage.Payload, stage.Payload);
                }
                else
                {
                    // stacking stage
                    //
                    stage.History = awaking.Stage;
                    updateQuery = Builders<Hibernation>.Update
                        .Set(h => h.Stage, stage);
                }
                
            }
        }

        public async Task<Hibernation> CreateOrUpdateAsync(string userId, string subjectName, string productName, StagePayload stage)
        {
            var filter = Builders<Hibernation>.Filter.Where(h => h.UserId == userId && h.SubjectName == subjectName && h.ProductName == productName);
            var updator = Builders<Hibernation>.Update
                .SetOnInsert(h => h.UserId, userId)
                .SetOnInsert(h => h.SubjectName, subjectName)
                .SetOnInsert(h => h.ProductName, productName)
                .Set(h => h.Stage, stage)
                .Set(h => h.UpdatedAt, DateTime.Now);

            Hibernation dormancy = await Context.GetCollection<Hibernation>()
                .FindOneAndUpdateAsync(filter, updator, new FindOneAndUpdateOptions<Hibernation>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });

            return dormancy;
        }
        
        public async Task<Hibernation> CreateOrUpdateAsync(Hibernation dormancy)
        {
            var filter = Builders<Hibernation>.Filter.Where(h => h.UserId == dormancy.UserId && h.SubjectName == dormancy.SubjectName && h.ProductName == dormancy.ProductName);
            var updator = Builders<Hibernation>.Update
                .SetOnInsert(h => h.UserId, dormancy.UserId)
                .SetOnInsert(h => h.SubjectName, dormancy.SubjectName)
                .SetOnInsert(h => h.ProductName, dormancy.ProductName)
                .Set(h => h.Stage, dormancy.Stage)
                .Set(h => h.UpdatedAt, DateTime.Now);

            Hibernation updatedDomancy = await Context.GetCollection<Hibernation>()
                .FindOneAndUpdateAsync(filter, updator, new FindOneAndUpdateOptions<Hibernation>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });

            return updatedDomancy;
        }
    }
}