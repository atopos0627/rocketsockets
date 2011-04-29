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
using rocketsockets.Config;

namespace rocketsockets.Impl
{
    public class SocketServer :
        ISocketServer,
        IDisposable
    {
        public IServerConfiguration Configuration { get; set; }
        public OnConnectionReceived OnConnection { get; set; }
        public bool Running { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public List<ISocketListener> Listeners { get; set; }
        public IListenerFactory ListenerFactory { get; set; }
        public IScheduler Scheduler { get; set; }
        
        public void OnSocket( ISocket socket ) 
        {
            var handle = new SocketHandle( 
                socket,
                Scheduler,
                (x, y) => Scheduler.QueueOperation( Operation.Generic, () => OnBytes( x, y ) ) 
            );
            OnConnection( handle );
        }

        public void Start( OnConnectionReceived onConnection, OnBytesReceived onBytes )
        {
            Running = true;
            OnConnection = onConnection;
            OnBytes = onBytes;
            Scheduler.Start();
            Listeners = Configuration
                .Endpoints
                .Select( endpoint =>
                {
                    var socket = ListenerFactory.CreateListener( Scheduler, endpoint, Configuration );
                    socket.ListenTo( OnSocket );
                    return socket;
                })
                .ToList();
        }

        public void Stop()
        {
            Running = false;
            Listeners.ForEach( x => x.Close() );
            Listeners.Clear();
            Scheduler.Stop();
        }

        public SocketServer( IServerConfiguration configuration, IListenerFactory listenerFactory, IScheduler scheduler )
        {
            Configuration = configuration;
            Scheduler = scheduler;
            ListenerFactory = listenerFactory;
        }

        public void Dispose()
        {
            if( Running )
                Stop();
        }
    }
}