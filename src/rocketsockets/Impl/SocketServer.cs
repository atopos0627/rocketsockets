using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketServer
        : ISocketServer
    {
        public IServerConfiguration Configuration { get; set; }
        public OnConnectionReceived OnConnection { get; set; }
        public bool Running { get; set; }
        public ISocketLoop EventLoop { get; set; }
        public IMailboxProcessor Mailboxes { get; set; }
        public List<Socket> Listeners { get; set; }
        
        public void Bind( IEndpointConfiguration configuration )
        {
            "Binding to endpoint {0}:{1}"
                .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );

            try
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
            catch (Exception e)
            {
                "Binding to endpoint {0}:{1} FAILED."
                    .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );
            }
        }

        public void ListenTo( Socket socket )
        {
            try
            {
                if( Running )
                {
                    "Listening to socket {0}"
                        .ToDebug<ISocketServer>( socket.LocalEndPoint.ToString() );
                    socket.BeginAccept( OnClient, socket );
                }
            }
            catch (Exception e)
            {
                "FAILURE while attempting to listen to socket {0}"
                    .ToDebug<ISocketServer>( socket.LocalEndPoint.ToString() );
            }
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                var listener = result.AsyncState as Socket;
                var socket = listener.EndAccept( result );
                "Socket connection on {0} to client @ {1}"
                    .ToDebug<ISocketServer>( listener.LocalEndPoint.ToString(), socket.RemoteEndPoint.ToString() );
                ListenTo( listener );
                OnSocket( socket );
            }
            catch ( Exception ex )
            {
                "Error occurred while establishing connection to client: \r\n\t {0}"
                    .ToDebug<ISocketServer>( ex );
            }
        }

        public void OnSocket( Socket socket ) 
        {
            var adapter = new SocketAdapter( socket, Configuration );
            var id = socket.RemoteEndPoint.ToString();
            var handle = new SocketHandle( id, adapter, EventLoop, (x, y) => Mailboxes.Write( x, y ) );
            adapter.AddCloseCallback( () => Mailboxes.Remove( id ) );
            OnConnection( id, handle );
        }

        public void Start( OnConnectionReceived onConnection, OnBytesReceived onBytes )
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