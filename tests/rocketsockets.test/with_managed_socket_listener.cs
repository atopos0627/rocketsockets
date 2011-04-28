using Machine.Specifications;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class with_managed_socket_listener : with_listener_setup
    {
        protected static IEventLoop loop;
        protected static ISocketListener listener;

        private Establish context = () => { 
                                              loop = new EventLoopStub();
                                              listener = new ManagedSocketListener( loop, testConfiguration.Endpoints[0], testConfiguration );
        };

        //Cleanup shutdown = () => listener.Dispose();
    }
}