using System;
using System.Net;
using System.Net.Sockets;

namespace rocketsockets
{
    public class EndpointListener
        : IEndpointListener
    {
        public IEndpointConfiguration Configuration { get; set; }
        public Socket Listener { get; set; }
        public Action<Socket> OnConnection { get; set; }
        public bool Running { get; set; }
        public IPEndPoint ServerEndpoint { get; set; }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                var socket = Listener.EndAccept( result );
                WaitForConnection();
                OnConnection( socket );
            }
            catch( SocketException sockEx )
            {
                Console.WriteLine( "WinSock sharted: {0}", sockEx );
            }
        }

        public void Start() 
        {
            Running = true;
            Listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            Listener.Bind( ServerEndpoint );
            Listener.Listen( 10000 );
            WaitForConnection();
        }

        public void Stop() 
        {
            Running = false;
        }

        public void WaitForConnection()
        {
            if(Running)
                Listener.BeginAccept( OnClient, null );
        }

        public EndpointListener( IEndpointConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}