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
using System.IO;
using System.Threading.Tasks;
using rocketsockets.Config;
using Symbiote.Core.Extensions;

namespace rocketsockets.Impl.Win32
{
    public unsafe class Win32SocketAdapter
        : ISocket
    {
        public string Id { get; set; }

        public IServerConfiguration Configuration { get; set; }
        public SOCKET Connection { get; set; }
        public Stream SocketStream { get; set; }
		
        public byte[] Bytes { get; set; }
        public bool Disposed { get; set; }
        public bool Listening { get; set; }
        public static long Total;
		
        public IAsyncResult ReadHandle { get; set; }
        public IAsyncResult WriteHandle { get; set; }
        public Task Listener { get; set; }

        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public List<Action> OnDisconnect { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action<ISocket> OnSocket { get; set; }
        public Action OnWriteCompleted { get; set; }

        public const int INVALID_SOCKET = -1;

        public void AddCloseCallback( Action onClose )
        {
            OnDisconnect.Add( onClose );
        }

        public SOCKET Bind( IEndpointConfiguration configuration )
        {
            SOCKET socket = INVALID_SOCKET;
            "Binding to endpoint {0}:{1}"
                .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );

            try
            {
                WSAPROTOCOL_INFO info = new WSAPROTOCOL_INFO();
                
                Native.WSASocket(
                        (int) AddressFamily.InterNetworkv4,
                        (int) SocketType.Stream,
                        (int) ProtocolType.Tcp,
                        out info,
                        0,
                        (int) SocketFlags.Overlapped
                    );

                var address = Native.inet_addr( configuration.AnyInterface 
                                  ? "0.0.0.0"
                                  : configuration.BindTo );
                var port = Native.htons( (ushort) configuration.Port );
                sockaddr_in socket_address = new sockaddr_in();
                socket_address.sin_addr.S_addr = address;
                socket_address.sin_port = port;
                socket_address.sin_family = (short) AddressFamily.InterNetworkv4;

                Native.bind( socket, &socket_address, sizeof ( sockaddr_in ) );
                Native.listen( socket, 10000 );
                return socket;
            }
            catch (Exception e)
            {
                "Binding to endpoint {0}:{1} FAILED."
                    .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );
            }
            return socket;
        }

        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                Listening = false;
                try
                {
                    if( WriteHandle != null && WriteHandle.AsyncWaitHandle != null && !WriteHandle.IsCompleted )
                        WriteHandle.AsyncWaitHandle.WaitOne();
					
                    if( SocketStream != null )
                    {
                        SocketStream.Flush();
                        SocketStream.Close();
                    }

                    if( ReadHandle != null && ReadHandle.AsyncWaitHandle != null && !ReadHandle.IsCompleted )
                    {
                        ReadHandle.AsyncWaitHandle.Close();
                        ReadHandle.AsyncWaitHandle.Dispose();
                    }

                    if( Listener != null && Listener.Status == TaskStatus.Running )
                        Listener.Dispose();

                    if( Connection != null )
                    {
                        //Connection.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.DontLinger, true );
                        //Connection.LingerState.Enabled = false;
                        //Connection.Close( -1 );
                        //var gch = GCHandle.Alloc( Connection );
                        //var sock = new SOCKET( GCHandle.ToIntPtr( gch ) );
                        //Native.closesocket( sock );
                        
                    }

                    OnDisconnect.ForEach( x => x() );
                    OnDisconnect.Clear();

                    OnBytes = null;
                    OnException = null;
                    OnWriteCompleted = null;
					
                    Bytes = new byte[0];
                }
                catch( Exception ex ) 
                {
                    Console.WriteLine( "poop" );
                }
                finally
                {
                    SocketStream = null;
                }
            }
        }

        public void Listen() 
        {
            try
            {
                if( Listening )
                {
                    "Listening to socket {0}"
                        .ToDebug<ISocketServer>(  );
                    // Connection.BeginAccept( OnClient, null );
                }
            }
            catch (Exception e)
            {
                //"FAILURE while attempting to listen to socket {0}"
                //    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
            }
        }

        public void ListenTo( Action<ISocket> onSocket )
        {
            Listening = true;
            OnSocket = onSocket;
            var task = Task.Factory.StartNew( Listen );
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                //var socket = Connection.EndAccept( result );
                Listen();
                //var id = socket.RemoteEndPoint.ToString();
                //var adapter = new DotNetSocketAdapter( socket, Configuration );
                //"Socket connection on {0} to client @ {1}"
                //    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString(), id );
                //OnSocket( adapter );
            }
            catch ( Exception ex )
            {
                "Error occurred while establishing connection to client: \r\n\t {0}"
                    .ToDebug<ISocketServer>( ex );
            }
        }

        public void OnRead( IAsyncResult result )
        {
            try
            {
                if ( !Disposed )
                {
                    var read = SocketStream.EndRead( result );
                    OnBytes( new ArraySegment<byte>( Bytes, 0, read ));
                }
            }
            catch ( Exception ex )
            {
                if ( OnException != null )
                    OnException( ex );
                Close();
            }
        }

        public void OnWrite( IAsyncResult result )
        {
            try
            {
                if( !Disposed )
                {
                    SocketStream.EndWrite( result );
                    OnWriteCompleted();
                }
                else if(WriteHandle != null && WriteHandle.AsyncWaitHandle != null )
                {
                    WriteHandle.AsyncWaitHandle.Dispose();
                }
            }
            catch( Exception ex )
            {
                if(WriteHandle != null && WriteHandle.AsyncWaitHandle != null )
                    WriteHandle.AsyncWaitHandle.Dispose();
                if ( OnException != null )
                    OnException( ex );
            }
        }

        public void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException )
        {
            try
            {
                if( !Disposed )
                {
                    OnBytes = onBytes;
                    OnException = onException;
                    ReadHandle = SocketStream.BeginRead( 
                        Bytes, 
                        0, 
                        Configuration.ReadBufferSize, 
                        OnRead, 
                        null );
                }
            }
            catch( Exception ex )
            {
                if ( OnException != null )
                    OnException( ex );
            }
        }

        public void Write( ArraySegment<byte> bytes, Action onComplete, Action<Exception> onException )
        {
            try
            {
                OnWriteCompleted = onComplete;
                OnException = onException;
                WriteHandle = SocketStream.BeginWrite( 
                    bytes.Array, 
                    bytes.Offset, 
                    bytes.Count, 
                    OnWrite, 
                    null );
            }
            catch ( IOException ioex )
            {
                if ( OnException != null )
                    OnException( ioex );
            }
            catch ( Exception ex )
            {
                if ( OnException != null )
                    OnException( ex );
            }
        }

        public Win32SocketAdapter( SOCKET connection, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                Configuration = configuration;
                Connection = connection;
                Bytes = new byte[configuration.ReadBufferSize];
                //SocketStream = new NetworkStream( connection );
                OnDisconnect = new List<Action>();
            } 
            catch (Exception ex) 
            {
                Console.WriteLine( ex );
            }
        }

        public Win32SocketAdapter( IEndpointConfiguration endpoint, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                Configuration = configuration;
                Connection = Bind( endpoint );
                Bytes = new byte[configuration.ReadBufferSize];
                //SocketStream = new NetworkStream( Connection );
                OnDisconnect = new List<Action>();
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
		
        ~Win32SocketAdapter()
        {
            //Console.WriteLine( "Remaining {1}: {0}", --Total, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
        }
    }
}