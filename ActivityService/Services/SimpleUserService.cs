using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class SimpleUserService
    {
        public IRepository<SimpleUser> Repository { get; }
        public SimpleUserService(IRepository<SimpleUser> repository)
        {
            Repository = repository;
        }

        public string Login(string userName)
        {
            Repository.GetAsync(userName);
        }
    }
}