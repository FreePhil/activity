namespace ActivityService.Services
{
    public interface ISubjectFetcherFactory
    {
        ISubjectFetcher Create(string userDomain);
    }
}