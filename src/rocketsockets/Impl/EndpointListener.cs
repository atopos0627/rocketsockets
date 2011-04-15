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
using System.Net;
using System.Net.Sockets;
using rocketsockets.Config;

namespace rocketsockets.Impl
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
            Listener = new Socket( System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.IP );
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