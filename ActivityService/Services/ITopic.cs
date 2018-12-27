using System;
using EasyNetQ;

namespace ActivityService.Services
{
    
    [Obsolete("Will move to test export module")]
    public interface ITopic
    {
        IBus Bus { get; }
        string Name { get; }
    }
}