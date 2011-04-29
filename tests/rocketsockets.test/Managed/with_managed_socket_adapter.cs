using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.Impl.Managed;

namespace rocketsockets.test
{
    public class with_managed_socket_adapter : with_managed_socket_listener
    {
        protected static ManagedSocketAdapter socket;
        protected static Socket actual_socket;
        protected static bool connectionReceived;
        protected static bool wasListening;
        protected static bool isClosed;
        protected static ISocket server_socket;

        private Establish context = () => 
        { 
            listener.ListenTo( x => 
            {
                connectionReceived = true;
                server_socket = x;
                x.Write(  new ArraySegment<byte>(Encoding.UTF8.GetBytes( "hi" )), () => { }, e => { } );
                x.AddCloseCallback( 
                    () => isClosed = true
                    );
            } );

            actual_socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            actual_socket.Connect( IPAddress.Parse( "127.0.0.1" ), 8998 );
            socket = new ManagedSocketAdapter( actual_socket, testConfiguration );
        };

        Cleanup cleanup =() =>
        {
            actual_socket.Close();
            server_socket.Close();
            listener.Close();
        };
    }
}