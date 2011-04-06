using System;
using hotstack.Owin;
using rocketsockets;

namespace hotstack.Transport.Socket
{
    public class ResponseWriter
        : IOwinObserver
    {
        public ISocketHandle Socket { get; set; }

        public bool OnNext( ArraySegment<byte> segment, Action continuation )
        {
            Socket.Write( segment, continuation, OnError );
            return true;
        }

        public void OnError( Exception exception )
        {
            Console.WriteLine( exception.Message );
        }

        public void OnComplete()
        {
            // Close();
        }

        public ResponseWriter( ISocketHandle connection )
        {
            Socket = connection;
        }
    }
}