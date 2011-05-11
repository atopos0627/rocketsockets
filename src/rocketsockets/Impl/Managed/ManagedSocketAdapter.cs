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
using System.Net.Sockets;
using rocketsockets.Config;
using Symbiote.Core.Extensions;

namespace rocketsockets.Impl.Managed
{
    public class ManagedSocketAdapter
        : ISocket
    {
        public string Id { get; set; }
        public IServerConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public Stream SocketStream { get; set; }
        public byte[] Bytes { get; set; }
        public bool Disposed { get; set; }
        public IAsyncResult ReadHandle { get; set; }
        public IAsyncResult WriteHandle { get; set; }
        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public List<Action> OnDisconnect { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action OnWriteCompleted { get; set; }

        public void AddCloseCallback( Action onClose )
        {
            OnDisconnect.Add( onClose );
        }

        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                try
                {
                    if( WriteHandle != null && !WriteHandle.IsCompleted )
                        WriteHandle.AsyncWaitHandle.WaitOne();

                    if( SocketStream != null )
                    {
                        SocketStream.Flush();
                        SocketStream.Close();
                    }

                    if( Connection != null )
                    {
                        Connection.LingerState.Enabled = false;
                        Connection.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.DontLinger, true );
                        Connection.Close( -1 );
                        Connection.Dispose();
                        Connection = null;
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
                    "An exception occurred when closing socket {0}. \r\n\t{1}"
                        .ToError<ISocketServer>( Id, ex );
                }
                finally
                {
                    SocketStream = null;
                }
            }
        }

        public void OnRead( IAsyncResult result )
        {
            try
            {
                if ( !Disposed )
                {
                    var read = SocketStream.EndRead( result );
                    var bytes = new byte[ read ];
                    Buffer.BlockCopy( Bytes, 0, bytes, 0, read );
                    OnBytes( new ArraySegment<byte>( bytes , 0, read ));
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
                    SocketStream.Flush();
                    OnWriteCompleted();
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

        public ManagedSocketAdapter( Socket connection, IServerConfiguration configuration )
        {
            Id = Guid.NewGuid().ToString();
            Configuration = configuration;
            Connection = connection;
            Bytes = new byte[configuration.ReadBufferSize];
            SocketStream = new NetworkStream( connection );
            OnDisconnect = new List<Action>();
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