using System;

namespace ActivityService.Services
{
    [Obsolete("Will move to test export module")]
    public interface IErrorLimit
    {
        int RetryCounter { get; }
    }
}