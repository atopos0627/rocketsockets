using rocketsockets.Config;
using rocketsockets.Impl;

namespace rocketsockets.test.Stubs
{
    public class ListenerStubFactory : IListenerFactory
    {
        public ISocketListener Listener { get; set; }

        public ISocketListener CreateListener( IScheduler scheduler, IEndpointConfiguration endpoint, IServerConfiguration server )
        {
            return Listener;
        }

        public ListenerStubFactory( ISocketListener listener ) 
        {
            Listener = listener;
        }
    }
}