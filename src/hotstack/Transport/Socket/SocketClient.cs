using System;
using hotstack.Owin.Impl;
using rocketsockets;

namespace hotstack.Transport.Socket
{
    public class SocketClient
        : IDisposable
    {
        public string Id { get; set; }
        public Request Request { get; set; }
        public IApplication Application { get; set; }
        public ISocketHandle Socket { get; set; }
        public Action<SocketClient, ArraySegment<byte>> Next { get; set; }

        public void Dispose()
        {
            Console.WriteLine( "Nuking socket client" );
            Request = null;
            Application = null;
            Socket = null;
            Next = null;
        }
    }
}