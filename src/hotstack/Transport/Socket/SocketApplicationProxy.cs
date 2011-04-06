using System;
using System.Collections.Concurrent;
using hotstack.Config;
using hotstack.Owin;
using hotstack.Owin.Impl;
using hotstack.Owin.Parser;
using rocketsockets;

namespace hotstack.Transport.Socket 
{
    public class SocketApplicationProxy :
        IApplicationAdapter
    {
        public ConcurrentDictionary<string, SocketClient> Clients { get; set; }
        public HttpServerConfiguration Configuration { get; set; }
        public IRouteRequest Router { get; set; }
        public IOwinObserver Writer { get; set; }

        public void AddSocket( string id, ISocketHandle socket )
        {
            Clients.AddOrUpdate( id, 
                x => new SocketClient() { Id = id, Socket = socket, Next = ParseRequest }, 
                ( x, y ) => new SocketClient() { Id = id, Socket = socket, Next = ParseRequest } );
            socket.Read();
        }

        public void HandleNextRead( string id, ArraySegment<byte> bytes )
        {
            SocketClient client = null;
            if( Clients.TryGetValue( id, out client ) )
            {
                client.Next( client, bytes );
                client.Socket.Read();
            }
        }

        public void ParseRequest( SocketClient client, ArraySegment<byte> bytes ) 
        {
            var request = new Request( x => HandleNextRead( client.Id, bytes ) );
            RequestParser.PopulateRequest( request, bytes );
            client.Request = request;
            client.Application = null;
            if( !client.Request.CanHaveBody && client.Request.HeadersComplete )
            {
                CreateApplication( client );
                client.Application.OnComplete();
            }
            client.Next = CreateApplication;
        }

        public void HandleRequestBody( SocketClient client, ArraySegment<byte> bytes ) 
        {
            client.Application.OnNext( bytes, () => client.Socket.Read() );
            if( client.Application.RequestCompleted ) 
            {
                client.Application.OnComplete();
                client.Next = ParseRequest;
            }
            else
            {
                client.Next = HandleRequestBody;
            }
        }

        public void CreateApplication( SocketClient client ) 
        {
            client.Application = Router.GetApplicationFor( client.Request );
            var responseHelper = new ResponseHelper( Configuration );
            var writer = new ResponseWriter( client.Socket );
            responseHelper.Setup( writer );

            client.Application.Process( 
                client.Request, 
                responseHelper,
                Console.WriteLine );
            
            client.Next = HandleRequestBody;
        }

        public void CreateApplication( SocketClient client, ArraySegment<byte> bytes ) 
        {
            CreateApplication( client );
            HandleNextRead( client.Id, bytes );
        }

        public SocketApplicationProxy( IRouteRequest factory, HttpServerConfiguration configuration )
        {
            Clients = new ConcurrentDictionary<string, SocketClient>();
            Router = factory;
            Configuration = configuration;
        }
    }
}
