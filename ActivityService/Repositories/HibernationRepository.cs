using System.Threading.Tasks;
using ActivityService.Models;
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
                .SingleAsync();
            
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
                var result = await Context.GetCollection<Hibernation>()
                    .UpdateOneAsync(h => h.UserId == userId && h.SubjectName == subjectName && h.ProductName == productName, updateQuery);
            }
        }
    }
}