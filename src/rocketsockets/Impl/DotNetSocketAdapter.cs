using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbiote.Core.Extensions;
using SocketError = System.Net.Sockets.SocketError;

namespace rocketsockets
{
    public class DotNetSocketAdapter
        : ISocket
    {
        public string Id { get; set; }

        public IServerConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public Stream SocketStream { get; set; }
		
        public byte[] Bytes { get; set; }
        public bool Disposed { get; set; }
        public bool Listening { get; set; }
        public static long Total;
		
        public IAsyncResult ReadHandle { get; set; }
        public IAsyncResult WriteHandle { get; set; }
        public Task Listener { get; set; }
        public IEventLoop ClientEventLoop { get; set; }

        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public List<Action> OnDisconnect { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action<ISocket> OnSocket { get; set; }
        public Action OnWriteCompleted { get; set; }

        public void AddCloseCallback( Action onClose )
        {
            OnDisconnect.Add( onClose );
        }

        public Socket Bind( IEndpointConfiguration configuration )
        {
            "Binding to endpoint {0}:{1}"
                .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );

            try
            {
                var socket = new Socket( 
                    System.Net.Sockets.AddressFamily.InterNetwork, 
                    System.Net.Sockets.SocketType.Stream,
                    System.Net.Sockets.ProtocolType.IP );
                var address = configuration.AnyInterface 
                                  ? IPAddress.Any 
                                  : IPAddress.Parse( configuration.BindTo );
                var endpoint = new IPEndPoint( address, configuration.Port );
                socket.Bind( endpoint );
                socket.Listen( 1000 );
                return socket;
            }
            catch (Exception e)
            {
                "Binding to endpoint {0}:{1} FAILED."
                    .ToDebug<ISocketServer>( configuration.BindTo ?? "0.0.0.0", configuration.Port );
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
                    //if( WriteHandle != null && WriteHandle.AsyncWaitHandle != null && !WriteHandle.IsCompleted )
                    //    WriteHandle.AsyncWaitHandle.WaitOne();
					
                    //if( ReadHandle != null && ReadHandle.AsyncWaitHandle != null && !ReadHandle.IsCompleted )
                    //{
                    //    ReadHandle.AsyncWaitHandle.Close();
                    //    ReadHandle.AsyncWaitHandle.Dispose();
                    //}

                    if( SocketStream != null )
                    {
                        SocketStream.Flush();
                        SocketStream.Close();
                    }

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

        public void HandleClient( Socket socket )
        {
            var adapter = new DotNetSocketAdapter( socket, Configuration );
            OnSocket( adapter );
        }

        public void Listen() 
        {
            try
            {
                if( Listening )
                {
                    //"Listening to socket {0}"
                    //    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
                    Connection.BeginAccept( OnClient, null );
                }
            }
            catch (Exception e)
            {
                "FAILURE while attempting to listen to socket {0}"
                    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
            }
        }

        public void ListenLoop()
        {
            while ( Listening )
            {
                try
                {
                    var client = Connection.Accept();
                    ClientEventLoop.Enqueue( () => HandleClient( client ) );
                }
                catch ( Exception )
                {
                    "FAILURE while attempting to listen to socket {0}"
                        .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
                }
            }
        }

        public void ListenTo( Action<ISocket> onSocket )
        {
            Listening = true;
            OnSocket = onSocket;
            var task = Task.Factory.StartNew( ListenLoop );
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                Listen();
                var socket = Connection.EndAccept( result );
                var adapter = new DotNetSocketAdapter( socket, Configuration );
                //"Socket connection on {0} to client @ {1}"
                //    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString(), id );
                OnSocket( adapter );
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
                    SocketStream.Flush();
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

        public DotNetSocketAdapter( Socket connection, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                //Id = connection.RemoteEndPoint.ToString();
                Id = Guid.NewGuid().ToString();
                Configuration = configuration;
                Connection = connection;
                Bytes = new byte[configuration.ReadBufferSize];
                SocketStream = new NetworkStream( connection );
                OnDisconnect = new List<Action>();
            } 
            catch (Exception ex) 
            {
                Console.WriteLine( ex );
            }
        }

        public DotNetSocketAdapter( IEventLoop clientLoop, IEndpointConfiguration endpoint, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                ClientEventLoop = clientLoop;
                Configuration = configuration;
                Connection = Bind( endpoint );
                Bytes = new byte[configuration.ReadBufferSize];
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
		
        ~DotNetSocketAdapter()
        {
            //Console.WriteLine( "Remaining {1}: {0}", --Total, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
        }
    }
}