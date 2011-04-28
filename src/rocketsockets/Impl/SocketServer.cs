// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Concurrency;

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
        public List<ISocketListener> Listeners { get; set; }
        
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
                    ISocketListener socket = new ManagedSocketListener( ClientEventLoop, x, Configuration );
                    socket.ListenTo( OnSocket );
                    return socket;
                })
                .ToList();
        }

        public void Stop()
        {
            Running = false;
            ReadEventLoop.Stop();
            WriteEventLoop.Stop();
            DisposeEventLoop.Stop();
            ClientEventLoop.Stop();
            ApplicationEventLoop.Stop();
            Listeners.ForEach( x => x.Close() );
            Listeners.Clear();
        }

        public SocketServer( IServerConfiguration configuration )
        {
            Configuration = configuration;
        }

        public void Dispose()
        {
            if( Running )
                Stop();
        }
    }
}