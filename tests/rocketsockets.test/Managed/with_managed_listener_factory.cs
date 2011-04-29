using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.Impl.Managed;

namespace rocketsockets.test
{
    public class with_managed_listener_factory : with_default_server_setup
    {
        protected static IListenerFactory factory;
        protected static SchedulerStub scheduler;

        private Establish context = () => 
        { 
            factory = new ManagedListenerFactory(); 
            scheduler = new SchedulerStub();
        };
    }
}