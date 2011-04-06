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
        IHandleReads
    {
        public ConcurrentDictionary<string, SocketClient> Clients { get; set; }
        public IRouteRequest Factory { get; set; }
        public HttpServerConfiguration Configuration { get; set; }
        
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
            client.Application = Factory.GetApplicationFor( client.Request );
            client.Application.Process( 
                client.Request, 
                new ResponseHelper( Configuration ),
                Console.WriteLine );
            client.Application.OnNext( bytes, () => client.Socket.Read() );
            client.Next = client.Application.RequestCompleted
                ? (Action<SocketClient, ArraySegment<byte>>) ParseRequest
                : HandleRequestBody;
        }

        public void CreateApplication( SocketClient client, ArraySegment<byte> bytes ) 
        {
            client.Application = Factory.GetApplicationFor( client.Request );
            client.Application.Process( 
                client.Request, 
                new ResponseHelper( Configuration ),
                Console.WriteLine );
            client.Next = HandleRequestBody;
            HandleNextRead( client.Id, bytes );
        }

        public SocketApplicationProxy( IRouteRequest factory, HttpServerConfiguration configuration )
        {
            Clients = new ConcurrentDictionary<string, SocketClient>();
            Factory = factory;
            Configuration = configuration;
        }
    }
}
