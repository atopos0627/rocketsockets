using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public delegate void OnBytesReceived( string receivedFrom, ArraySegment<byte> segment );

    public class SocketServer
        : ISocketServer
    {
        public IServerConfiguration Configuration { get; set; }
        public Action<string, ISocketHandle> OnConnection { get; set; }
        public bool Running { get; set; }
        public ISocketLoop EventLoop { get; set; }
        public IMailboxProcessor Mailboxes { get; set; }
        public List<Socket> Listeners { get; set; }
        
        public void Bind( IEndpointConfiguration configuration )
        {
            var socket = new Socket( 
                AddressFamily.InterNetwork, 
                SocketType.Stream,
                ProtocolType.IP );
            var address = configuration.AnyInterface 
                ? IPAddress.Any 
                : IPAddress.Parse( configuration.BindTo );
            var endpoint = new IPEndPoint( address, configuration.Port );
            socket.Bind( endpoint );
            socket.Listen( 10000 );
            ListenTo( socket );
        }

        public void ListenTo( Socket socket )
        {
            if( Running )
                socket.BeginAccept( OnClient, socket );
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                var listener = result.AsyncState as Socket;
                if( listener != null )
                {
                    var socket = listener.EndAccept( result );
                    ListenTo( listener );
                    OnSocket( socket );
                }
            }
            catch ( Exception ex )
            {
                
            }
        }

        public void OnSocket( Socket socket ) 
        {
            var adapter = new SocketAdapter( socket, Configuration );
            var id = socket.RemoteEndPoint.ToString();
            var handle = EventLoop.AddSocket( id, adapter, (x, y) => Mailboxes.Write( x, y ) );
            OnConnection( id, handle );
        }

        public void Start( Action<string, ISocketHandle> onConnection, OnBytesReceived onBytes )
        {
            Running = true;
            OnConnection = onConnection;
            Mailboxes = new MailboxProcessor( onBytes );
            EventLoop = new SocketEventLoop();
            EventLoop.Start();
            Mailboxes.Start();
            Configuration
                .Endpoints
                .ForEach( Bind );
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
            Listeners = new List<Socket>( configuration.Endpoints.Count );
        }
    }
}