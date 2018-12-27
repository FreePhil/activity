using System;
using ActivityService.Models;

namespace ActivityService.Services
{
    [Obsolete("Will move to test export module")]
    public class ActivityErrorLimit: IErrorLimit
    {
        public int RetryCounter
        {
            get { return 3; }
        }
    }
}