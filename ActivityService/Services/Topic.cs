using EasyNetQ;

namespace ActivityService.Services
{
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