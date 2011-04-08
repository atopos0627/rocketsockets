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

        public Action AddSocket( ISocketHandle socket )
        {
            var id = socket.Id;
            Clients.AddOrUpdate( id, 
                x => new SocketClient() { Id = id, Socket = socket, Next = ParseRequest }, 
                ( x, y ) => new SocketClient() { Id = id, Socket = socket, Next = ParseRequest } );
            socket.Read();
            return () => HandleSocketClose( id );
        }

        public void CreateApplication( SocketClient client ) 
        {
            client.Application = Router.GetApplicationFor( client.Request );
            var responseHelper = new ResponseHelper( Configuration );
            var writer = new ResponseWriter( client );
            responseHelper.Setup( writer );

            client.Application.Process( 
                client.Request, 
                responseHelper,
                Console.WriteLine );
            
            client.Next = ParseRequest;
        }

        public void CreateApplication( SocketClient client, ArraySegment<byte> bytes ) 
        {
            CreateApplication( client );
            HandleNextRead( client.Id, bytes );
        }

        public void HandleNextRead( string id, ArraySegment<byte> bytes )
        {
            SocketClient client = null;
            if( Clients.TryGetValue( id, out client ) )
            {
                client.Next( client, bytes );
            }
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
                client.Socket.Read();
            }
        }

        public void HandleSocketClose( string id )
        {
            SocketClient client = null;
            Clients.TryRemove( id, out client );
        }

        public void ParseRequest( SocketClient client, ArraySegment<byte> bytes ) 
        {
            client.Next = ( client.Request == null || !client.Request.HeadersComplete ) 
                ? client.Next 
                : CreateApplication;

            client.Request = client.Request ?? new Request( x => HandleNextRead( client.Id, x ) );
            RequestParser.PopulateRequest( client.Request, bytes );

            if( !client.Request.CanHaveBody && client.Request.HeadersComplete )
            {
                CreateApplication( client );
                client.Application.OnComplete();
            }
        }

        public SocketApplicationProxy( IRouteRequest factory, HttpServerConfiguration configuration )
        {
            Clients = new ConcurrentDictionary<string, SocketClient>();
            Router = factory;
            Configuration = configuration;
        }
    }
}
