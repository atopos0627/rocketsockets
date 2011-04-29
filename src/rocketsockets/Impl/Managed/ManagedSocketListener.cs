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
using System.Threading.Tasks;
using rocketsockets.Config;
using Symbiote.Core.Extensions;

namespace rocketsockets.Impl
{
    public class ManagedSocketListener
        : ISocketListener
    {
        public Task Listener { get; set; }
        public IScheduler Scheduler { get; set; }
        public IServerConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public bool Disposed { get; set; }
        public bool Listening { get; set; }
        public Action<ISocket> OnSocket { get; set; }

        public Socket Bind( IEndpointConfiguration configuration )
        {
            try
            {
                var socket = new Socket( 
                    AddressFamily.InterNetwork, 
                    SocketType.Stream,
                    ProtocolType.IP );
                var address = configuration.AnyInterface 
                                  ? IPAddress.Any 
                                  : IPAddress.Parse( configuration.BindTo );
                var endpoint = new IPEndPoint( address, configuration.Port );
                socket.Bind( endpoint );
                socket.Listen( 1000 );
                return socket;
            }
            catch ( Exception ex )
            {
                "Binding to endpoint {0}:{1} FAILED."
                    .ToError<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );
                throw ex;
            }
            return null;
        }

        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                Listening = false;
                try
                {
                    if( Listener != null && Listener.Status == TaskStatus.Running )
                        Listener.Dispose();

                    if( Connection != null )
                    {
                        Connection.LingerState.Enabled = false;
                        Connection.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.DontLinger, true );
                        Connection.Close( -1 );
                        Connection.Dispose();
                        Connection = null;
                    }
                    OnSocket = null;
                }
                catch( Exception ex ) 
                {
                    "An exception occurred when closing a listening socket. \r\n\t{1}"
                        .ToError<ISocketServer>( ex );
                }
                finally
                {
                }
            }
        }

        public void HandleClient( Socket socket )
        {
            var adapter = new ManagedSocketAdapter( socket, Configuration );
            OnSocket( adapter );
        }
        
        public void ListenLoop()
        {
            while ( Listening )
            {
                try
                {
                    var client = Connection.Accept();
                    Scheduler.QueueOperation( Operation.Connect, () => HandleClient( client ) );
                }
                catch ( Exception ex )
                {
                    if( Connection != null )
                    {
                        "FAILURE while attempting to listen to socket {0}. \r\n\t{1}"
                            .ToError<ISocketServer>( Connection.LocalEndPoint.ToString(), ex );
                    }
                    else
                    {
                        "An exception occurred in Managed Socket Listener; probably due to unexpected shut-down. \r\n\t{0}"
                            .ToError<ISocketServer>( ex );
                    }
                }
            }
        }

        public void ListenTo( Action<ISocket> onSocket )
        {
            Listening = true;
            OnSocket = onSocket;
            var task = Task.Factory.StartNew( ListenLoop );
        }

        public ManagedSocketListener( IScheduler scheduler, IEndpointConfiguration endpoint, IServerConfiguration configuration )
        {
            try 
            {
                Scheduler = scheduler;
                Configuration = configuration;
                Connection = Bind( endpoint );
            }
            catch (Exception ex) 
            {
                Console.WriteLine( ex );
            }
        }

        public void Dispose()
        {
            if( !Disposed )
            {
                Close();
            }
        }
    }
}