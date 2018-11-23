using System;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IErrorLimit
    {
        int RetryCounter { get; }
    }
}