using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using rocketsockets.Config;
using Symbiote.Core.Extensions;

namespace rocketsockets.Impl
{
    public class ManagedSocketListener
        : ISocketListener
    {
        public Task Listener { get; set; }
        public IEventLoop ClientEventLoop { get; set; }
        public IServerConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public bool Disposed { get; set; }
        public bool Listening { get; set; }
        public Action<ISocket> OnSocket { get; set; }

        public Socket Bind( IEndpointConfiguration configuration )
        {
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
                socket.Listen( 1000 );
                return socket;
            }
            catch ( Exception ex )
            {
                "Binding to endpoint {0}:{1} FAILED."
                    .ToError<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );
                throw ex;
            }
            return null;
        }

        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                Listening = false;
                try
                {
                    if( Listener != null && Listener.Status == TaskStatus.Running )
                        Listener.Dispose();

                    if( Connection != null )
                    {
                        Connection.LingerState.Enabled = false;
                        Connection.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.DontLinger, true );
                        Connection.Close( -1 );
                        Connection.Dispose();
                        Connection = null;
                    }
                    OnSocket = null;
                }
                catch( Exception ex ) 
                {
                    "An exception occurred when closing a listening socket. \r\n\t{1}"
                        .ToError<ISocketServer>( ex );
                }
                finally
                {
                }
            }
        }

        public void HandleClient( Socket socket )
        {
            var adapter = new ManagedSocketAdapter( socket, Configuration );
            OnSocket( adapter );
        }
        
        public void Listen() 
        {
            try
            {
                if( Listening )
                {
                    Connection.BeginAccept( OnClient, null );
                }
            }
            catch ( Exception ex )
            {
                "FAILURE while attempting to listen to socket {0}. \r\n\t{1}"
                    .ToError<ISocketServer>( Connection.LocalEndPoint.ToString(), ex );
            }
        }

        public void ListenLoop()
        {
            while ( Listening )
            {
                try
                {
                    var client = Connection.Accept();
                    ClientEventLoop.Enqueue( () => HandleClient( client ) );
                }
                catch ( Exception ex )
                {
                    "FAILURE while attempting to listen to socket {0}. \r\n\t{1}"
                        .ToError<ISocketServer>( Connection.LocalEndPoint.ToString(), ex );
                }
            }
        }

        public void ListenTo( Action<ISocket> onSocket )
        {
            Listening = true;
            OnSocket = onSocket;
            var task = Task.Factory.StartNew( ListenLoop );
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                Listen();
                var socket = Connection.EndAccept( result );
                var adapter = new ManagedSocketAdapter( socket, Configuration );
                OnSocket( adapter );
            }
            catch ( Exception ex )
            {
                "Error occurred while establishing connection to client: \r\n\t {0}"
                    .ToError<ISocketServer>( ex );
            }
        }

        public ManagedSocketListener( IEventLoop clientLoop, IEndpointConfiguration endpoint, IServerConfiguration configuration )
        {
            ClientEventLoop = clientLoop;
            Configuration = configuration;
            Connection = Bind( endpoint );
        }

        public void Dispose()
        {
            if( !Disposed )
            {
                Close();
            }
        }
    }
}