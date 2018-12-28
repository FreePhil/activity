using System;
using System.Collections.Generic;
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
            catch (KeyNotFoundException e)
            {
                user = new SimpleUser {Name = userName};
                await Repository.AddAsync(user);
            }

            return user;
        }
    }
}