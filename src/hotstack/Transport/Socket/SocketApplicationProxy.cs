using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using hotstack.Config;
using hotstack.Owin;
using hotstack.Owin.Impl;
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
                x => new SocketClient() { Id = id, Socket = socket }, 
                ( x, y ) => new SocketClient() { Id = id, Socket = socket } );
            socket.Read();
        }

        public void HandleNextRead( string id, ArraySegment<byte> bytes )
        {
            SocketClient client = null;
            if( Clients.TryGetValue( id, out client ) )
            {
                client.Next( client, bytes );
            }
        }

        public void ParseRequest( SocketClient client, ArraySegment<byte> bytes ) 
        {
            var request = new Request( x => HandleNextRead( client.Id, bytes ) );
            request.Parse( request, bytes );
            client.Request = request;
            client.Application = null;
            client.Next = CreateApplication;
        }

        public void HandleRequestBody( SocketClient client, ArraySegment<byte> bytes ) 
        {
            client.Application.OnNext( bytes, () => client.Socket.Read() );
            client.Next = client.Application.RequestCompleted
                ? (Action<SocketClient, ArraySegment<byte>>) ParseRequest
                : HandleRequestBody;
        }

        public void CreateApplication( SocketClient client, ArraySegment<byte> bytes ) 
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
