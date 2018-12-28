using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class SimpleUserService: ISimpleUserService
    {
        private ISimpleUserRepository Repository { get; }
        public SimpleUserService(ISimpleUserRepository repository)
        {
            Repository = repository;
        }

        public async Task<SimpleUser> LoginAsync(string userName)
        {
            SimpleUser user;
            try
            {
                user = await Repository.GetByUserNameAsync(userName);
            }
            catch (Exception e)
            {
                user = new SimpleUser {Name = userName};
                Repository.AddAsync(user);
            }

            return user;
        }
    }
}