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
            if( Client.Request.Version == "1.0" )
                Client.Socket.Close();
        }

        public ResponseWriter( SocketClient client )
        {
            Client = client;
        }
    }
}