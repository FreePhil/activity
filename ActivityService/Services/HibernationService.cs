using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class HibernationService: IHibernationService
    {
        private IHibernationRepository Repository { get; }
        public HibernationService(IHibernationRepository repository)
        {
            Repository = repository;
        }

        public Task<Hibernation> GetHibernationAsync(string id)
        {
            return Repository.GetAsync(id);
        }

        public Task<Hibernation> GetHibernationAsync(string userId, string subjectName, string productName)
        {
            return Repository.GetAsync(userId, subjectName, productName);
        }

        public Task<Hibernation> CreateOrUpdateHibernationAsync(Hibernation dormancy)
        {
            return Repository.CreateOrUpdateAsync(dormancy);
        }

        public async Task<Hibernation> UpdateOnTheSameStageAsync(string id, StagePayload stageOnly)
        {
            var dormancy = await GetHibernationAsync(id);

            if (dormancy.Stage.Name == stageOnly.Name)
            {
                dormancy.Stage.Payload = stageOnly.Payload;
                dormancy.UpdatedAt = DateTime.Now;
                
                return await Repository.CreateOrUpdateAsync(dormancy);
            }

            return null;
        }
    }
}