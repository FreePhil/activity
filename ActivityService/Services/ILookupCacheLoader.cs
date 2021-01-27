namespace ActivityService.Services
{
    public interface ILookupCacheLoader
    {
        void ReadCache(string version);
    }
}