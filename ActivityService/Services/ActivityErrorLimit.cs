using System;
using ActivityService.Models;

namespace ActivityService.Services
{
    public class ActivityErrorLimit: IErrorLimit
    {
        public int RetryCounter
        {
            get { return 3; }
        }
    }
}