using EasyNetQ;

namespace ActivityService.Services
{
    public interface ITopic
    {
        IBus Bus { get; }
        string Name { get; }
    }
}