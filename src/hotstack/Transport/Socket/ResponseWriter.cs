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
            var keepAlive = Client.Request.KeepAlive;
            Client.Request = null;
            Client.Application = null;
            if( keepAlive )
                Client.Socket.Read();
            else
                Client.Socket.Close();
        }

        public ResponseWriter( SocketClient client )
        {
            Client = client;
        }
    }
}