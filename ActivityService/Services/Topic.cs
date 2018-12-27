using System;
using EasyNetQ;

namespace ActivityService.Services
{
    [Obsolete("Will move to test export module")]
    public class Topic: ITopic
    {
        public Topic(IBus bus)
        {
            Bus = bus;
            Name = "testbank_activities";
        }

        public IBus Bus { get; private set; }
        public string Name { get; private set; }
    }
}