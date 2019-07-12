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

        public async Task<Hibernation> CreateOrUpdateHibernationForwardAsync(Hibernation dormancy)
        {
            var frozen = await GetHibernationAsync(dormancy.UserId, dormancy.SubjectName, dormancy.ProductName);
            if (frozen == null)
            {
                dormancy.Stage.History = null;
                frozen = dormancy;
            }
            else
            {
                dormancy.Stage.History = frozen.Stage;
                frozen = dormancy;
            }
            
            return await Repository.CreateOrUpdateAsync(frozen);
        }
        
        public async Task<Hibernation> CreateOrUpdateHibernationBackwardAsync(Hibernation dormancy)
        {
            var frozen = await GetHibernationAsync(dormancy.UserId, dormancy.SubjectName, dormancy.ProductName);
            if (frozen != null)
            {
                while (frozen.Stage.History != null)
                {
                    if (frozen.Stage.Name != dormancy.Stage.Name)
                    {
                        frozen.Stage = frozen.Stage.History;
                    }
                    else
                    {
                        break;
                    }
                }

                if (frozen.Stage.Name == dormancy.Stage.Name)
                {
                    frozen.Stage.Payload = dormancy.Stage.Payload;
                }
            }
            else
            {
                return null;
            }
            
            return await Repository.CreateOrUpdateAsync(frozen);
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

        public async Task DeleteHibernationAsync(string id)
        {
            await Repository.DeleteAsync(id);
            return;
        }
    }
}