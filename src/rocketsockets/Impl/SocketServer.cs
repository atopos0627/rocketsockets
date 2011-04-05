using System;
using System.Net.Sockets;

namespace rocketsockets
{
    public class SocketServer
        : ISocketServer
    {
        public IServerConfiguration Configuration { get; set; }
        public Action<ISocket> OnConnection { get; set; }
        public bool Running { get; set; }
        public ISocketLoop Loop { get; set; }
        public IMailboxProcessor Mailboxes { get; set; }
        
        public void OnSocket( Socket socket ) 
        {
            var adapter = new SocketAdapter( socket, Configuration );
            var handle = Loop.AddSocket( adapter );
            OnConnection( handle );
        }

        public void Start( Action<ISocket> onConnection, Action<ArraySegment<byte>> onData )
        {
            Running = true;
            OnConnection = onConnection;
            Mailboxes = new MailboxProcessor( onData );
            Loop.Start();
            Mailboxes.Start();
        }

        public void Stop()
        {
            Running = false;
            Loop.Stop();
            Mailboxes.Stop();
        }

        public SocketServer( IServerConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}