using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEventLoop SocketEventLoop { get; set; }
        public IEventLoop DisposeEventLoop { get; set; }
        public IEventLoop ApplicationEventLoop { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public List<ISocket> Listeners { get; set; }
        
        public void OnSocket( ISocket socket ) 
        {
            var handle = new SocketHandle( 
                socket,
                SocketEventLoop, 
                DisposeEventLoop,
                (x, y) => ApplicationEventLoop.Enqueue( () => OnBytes( x, y ) ) 
            );
            OnConnection( handle );
        }

        public void Start( OnConnectionReceived onConnection, OnBytesReceived onBytes )
        {
            Running = true;
            OnConnection = onConnection;
            OnBytes = onBytes;
            SocketEventLoop = new EventLoop();
            ApplicationEventLoop = new EventLoop();
            DisposeEventLoop = new EventLoop();
            SocketEventLoop.Start( 1 );
            ApplicationEventLoop.Start( 1 );
            //DisposeEventLoop.Start( 1 );
            Listeners = Configuration
                .Endpoints
                .Select( x =>
                {
                    ISocket socket = new DotNetSocketAdapter( x, Configuration );
                    //ISocket socket = new Win32SocketAdapter( x, Configuration );
                    socket.ListenTo( OnSocket );
                    return socket;
                })
                .ToList();
        }

        public void Stop()
        {
            Running = false;
            SocketEventLoop.Stop();
            ApplicationEventLoop.Stop();
            Listeners.ForEach( x => x.Close() );
            Listeners.Clear();
        }

        public SocketServer( IServerConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}