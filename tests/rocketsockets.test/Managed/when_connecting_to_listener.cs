using System.Net.Sockets;
using System.Threading;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_connecting_to_listener : with_managed_socket_listener
    {
        static bool connectionReceived;
        static bool wasListening;
        static bool isClosed;
        static ISocket socket;
        static ManualResetEvent waitHandle;

        private Because of = () => { 
                                       waitHandle = new ManualResetEvent( false );
                                       listener.ListenTo( x => 
                                       {
                                           connectionReceived = true;
                                           socket = x;
                                           x.AddCloseCallback( 
                                               () => isClosed = true
                                               );
                                           waitHandle.Set();
                                           socket.Close();
                                       } );

                                       using( var client = new TcpClient() ) 
                                       {
                                           client.Connect( "localhost", 8998 );
                                           waitHandle.WaitOne();
                                           client.Close();
                                       }

                                       wasListening = listener.Listening;
                                       listener.Close();
        };

        private It should_have_listened = () => wasListening.ShouldBeTrue();
        private It should_have_marked_connection_as_received = () => connectionReceived.ShouldBeTrue();
        private It should_be_closed = () => isClosed.ShouldBeTrue();
        private It should_have_stopped = () => listener.Listening.ShouldBeFalse();
    }
}