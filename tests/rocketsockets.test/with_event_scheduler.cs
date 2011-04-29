using Machine.Specifications;
using rocketsockets.Impl;

namespace rocketsockets.test
{
    public class with_event_scheduler
    {
        public static EventLoopScheduler scheduler;

        private Establish context = () => 
        { 
            scheduler = new EventLoopScheduler();
        };
    }
}
