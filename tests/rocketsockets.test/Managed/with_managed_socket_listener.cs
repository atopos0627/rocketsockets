using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.Impl.Managed;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class with_managed_socket_listener : with_listener_setup
    {
        protected static IScheduler scheduler;
        protected static ISocketListener listener;

        private Establish context = () => { 
                                              scheduler = new SchedulerStub();
                                              listener = new ManagedSocketListener( scheduler, testConfiguration.Endpoints[0], testConfiguration );
        };
    }
}