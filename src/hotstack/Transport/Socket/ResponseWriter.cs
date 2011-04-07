using System;
using hotstack.Owin;
using rocketsockets;

namespace hotstack.Transport.Socket
{
    public class ResponseWriter
        : IOwinObserver
    {
        public SocketClient Client { get; set; }

        public bool OnNext( ArraySegment<byte> segment, Action continuation )
        {
            Client.Socket.Write( segment, continuation, OnError );
            return true;
        }

        public void OnError( Exception exception )
        {
            Console.WriteLine( exception.Message );
        }

        public void OnComplete()
        {
            var keep_alive = Client.Request.Version == "1.0" || Client.Request.Headers["Connection"].Equals( "Keep-Alive" );
            Client.Request = null;
            Client.Application = null;
            if( keep_alive )
                Client.Socket.Close();
            else
                Client.Socket.Read();
        }

        public ResponseWriter( SocketClient client )
        {
            Client = client;
        }
    }
}