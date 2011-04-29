using Machine.Specifications;
using rocketsockets.Impl;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class with_socket_handle
    {
        protected static ISocketHandle handle;
        protected static SocketStub socket;
        protected static OnBytesReceived onBytesReceived;
        protected static bool bytesRead;
        protected static bool underlyingSocketClosed;

        private Establish context = () => { 
                                              socket = new SocketStub() { Id  = "stub" };
                                              socket.AddCloseCallback( () => underlyingSocketClosed = true );
                                              onBytesReceived = ( id, bytes ) => bytesRead = true;
                                              handle = new SocketHandle( socket, new SchedulerStub(), onBytesReceived );
        };
    }
}