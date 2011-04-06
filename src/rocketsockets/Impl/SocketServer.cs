using System;
using System.Net.Sockets;

namespace rocketsockets
{
    public delegate void OnBytesReceived( string receivedFrom, ArraySegment<byte> segment );

    public class SocketServer
        : ISocketServer
    {
        public IServerConfiguration Configuration { get; set; }
        public Action<ISocketHandle> OnConnection { get; set; }
        public bool Running { get; set; }
        public ISocketLoop EventLoop { get; set; }
        public IMailboxProcessor Mailboxes { get; set; }
        
        public void OnSocket( Socket socket ) 
        {
            var adapter = new SocketAdapter( socket, Configuration );
            var id = socket.RemoteEndPoint.ToString();
            var handle = EventLoop.AddSocket( id, adapter, (x, y) => Mailboxes.Write( x, y ) );
            OnConnection( handle );
        }

        public void Start( Action<ISocketHandle> onConnection, OnBytesReceived onBytes )
        {
            Running = true;
            OnConnection = onConnection;
            Mailboxes = new MailboxProcessor( onBytes );
            EventLoop = new SocketEventLoop();
            EventLoop.Start();
            Mailboxes.Start();
        }

        public void Stop()
        {
            Running = false;
            EventLoop.Stop();
            Mailboxes.Stop();
        }

        public SocketServer( IServerConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}