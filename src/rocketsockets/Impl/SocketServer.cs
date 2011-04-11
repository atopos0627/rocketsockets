using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketServer :
        ISocketServer,
        IDisposable
    {
        public IServerConfiguration Configuration { get; set; }
        public OnConnectionReceived OnConnection { get; set; }
        public bool Running { get; set; }
        public IEventLoop ReadEventLoop { get; set; }
        public IEventLoop WriteEventLoop { get; set; }
        public IEventLoop ClientEventLoop { get; set; }
        public IEventLoop DisposeEventLoop { get; set; }
        public IEventLoop ApplicationEventLoop { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public List<ISocket> Listeners { get; set; }
        
        public ushort MakeWord ( byte pValueLow, byte pValueHigh )
        {
            var array = new [] { pValueLow, pValueHigh };
            return BitConverter.ToUInt16( array, 0 );
        }

        public void OnSocket( ISocket socket ) 
        {
            var handle = new SocketHandle( 
                socket,
                ReadEventLoop,
                WriteEventLoop,
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
            ReadEventLoop = new EventLoop();
            WriteEventLoop = new EventLoop();
            ClientEventLoop = new EventLoop();
            ApplicationEventLoop = new EventLoop();
            DisposeEventLoop = new EventLoop();
            ReadEventLoop.Start( 1 );
            WriteEventLoop.Start( 1 );
            ApplicationEventLoop.Start( 1 );
            DisposeEventLoop.Start( 1 );
            ClientEventLoop.Start( 1 );
            Listeners = Configuration
                .Endpoints
                .Select( x =>
                {
                    ISocket socket = new DotNetSocketAdapter( ClientEventLoop, x, Configuration );
                    //ISocket socket = new Win32SocketAdapter( x, Configuration );
                    socket.ListenTo( OnSocket );
                    return socket;
                })
                .ToList();
        }

        public void Stop()
        {
            // Native.WSACleanup();
            Running = false;
            ReadEventLoop.Stop();
            ApplicationEventLoop.Stop();
            Listeners.ForEach( x => x.Close() );
            Listeners.Clear();
        }

        public SocketServer( IServerConfiguration configuration )
        {
            Configuration = configuration;
            var data = new WSAData() { highVersion = 2, version = 2 };
            // Native.WSAStartup( MakeWord( 2, 0 ), out data );
        }

        public void Dispose()
        {
            if( Running )
                Stop();
        }
    }
}