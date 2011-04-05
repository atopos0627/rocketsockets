﻿using System;
using System.IO;
using System.Net.Sockets;

namespace rocketsockets
{
    public class SocketAdapter
        : ISocket
    {
        public IServerConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public Stream SocketStream { get; set; }
		
        public byte[] Bytes { get; set; }
        public bool Disposed { get; set; }
        public static long Total;
		
        public IAsyncResult ReadHandle { get; set; }
        public IAsyncResult WriteHandle { get; set; }

        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public Action OnDisconnect { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action OnWriteCompleted { get; set; }
		
        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                try
                {
                    if( WriteHandle != null && WriteHandle.AsyncWaitHandle != null && !WriteHandle.IsCompleted )
                        WriteHandle.AsyncWaitHandle.WaitOne();
					
                    if( SocketStream != null )
                        SocketStream.Close();

                    if( Connection != null )
                        Connection.Close();

                    if( ReadHandle != null && ReadHandle.AsyncWaitHandle != null )
                        ReadHandle.AsyncWaitHandle.Dispose();
					
                    OnBytes = null;
                    OnException = null;
                    OnWriteCompleted = null;
					
                    Bytes = new byte[0];
                }
                finally
                {
                    SocketStream = null;
                    Connection = null;
                }
            }
        }

        public void OnRead( IAsyncResult result )
        {
            var read = SocketStream.EndRead( result );
            OnBytes( new ArraySegment<byte>( Bytes, 0, read ));
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
                OnException( ex );
            }
        }

        public void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException )
        {
            try
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
            catch( Exception ex )
            {
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
                OnException( ioex );
            }
            catch ( Exception ex )
            {
                OnException( ex );
            }
        }

        public SocketAdapter( Socket connection, IServerConfiguration configuration )
        {
            try 
            {
                Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                Configuration = configuration;
                Connection = connection;
                Bytes = new byte[configuration.ReadBufferSize];
                SocketStream = new NetworkStream( connection );
            } 
            catch (Exception ex) 
            {
                Console.WriteLine( ex );
            }
        }
		
        public void Check()
        {
            SocketStream.BeginWrite( new byte[] {}, 0, 0, CheckComplete, null );
        }
		
        public void CheckComplete( IAsyncResult result )
        {
            try 
            {
                SocketStream.EndWrite( result );
            } 
            finally
            {
                Close();
                if( OnDisconnect != null )
                {
                    OnDisconnect();
                    OnDisconnect = null;
                }
            }
        }
		
        public void Dispose()
        {
            if( !Disposed )
            {
                Close();
            }
        }
		
        ~SocketAdapter()
        {
            Console.WriteLine( "Remaining {1}: {0}", --Total, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
        }
    }
}