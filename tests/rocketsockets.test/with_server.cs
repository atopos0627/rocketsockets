using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.test.Stubs;

namespace rocketsockets.test
{
    public class with_server : with_default_server_setup
    {
        protected static ISocketServer server;
        protected static SchedulerStub scheduler;
        protected static ListenerStub listener;
        protected static SocketStub socket;
        protected static ListenerStubFactory listenerFactory;


        private Establish context = () => { 

            listener = new ListenerStub();
            listenerFactory = new ListenerStubFactory( listener );
            scheduler = new SchedulerStub();
            socket = new SocketStub();

            server = new SocketServer( testConfiguration, listenerFactory, scheduler );
        };

        Cleanup shutdown = () => { server.Stop(); };
    }
}
