using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class Win32SocketAdapter
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
                    if( WriteHandle != null && WriteHandle.AsyncWaitHandle != null && !WriteHandle.IsCompleted )
                        WriteHandle.AsyncWaitHandle.WaitOne();
					
                    if( ReadHandle != null && ReadHandle.AsyncWaitHandle != null )
                    {
                        ReadHandle.AsyncWaitHandle.Close();
                        ReadHandle.AsyncWaitHandle.Dispose();
                    }

                    if( Listener != null && Listener.Status == TaskStatus.Running )
                        Listener.Dispose();

                    if( SocketStream != null )
                        SocketStream.Close();

                    if( Connection != null )
                    {
                        var gch = GCHandle.Alloc( Connection );
                        var nativeSoquette = GCHandle.ToIntPtr( gch );
                        var sock = (SOCKET) nativeSoquette.ToInt32();
                        Native.closesocket( sock );
                        //Connection.Close();
                    }

                    OnDisconnect.ForEach( x => x() );
                    OnDisconnect.Clear();

                    OnBytes = null;
                    OnException = null;
                    OnWriteCompleted = null;
					
                    Bytes = new byte[0];
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
                        .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
                    Connection.BeginAccept( OnClient, null );
                }
            }
            catch (Exception e)
            {
                "FAILURE while attempting to listen to socket {0}"
                    .ToDebug<ISocketServer>( Connection.LocalEndPoint.ToString() );
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
                var listener = result.AsyncState as Socket;
                var socket = listener.EndAccept( result );
                Listen();
                var id = socket.RemoteEndPoint.ToString();
                var adapter = new DotNetSocketAdapter( socket, Configuration );
                "Socket connection on {0} to client @ {1}"
                    .ToDebug<ISocketServer>( listener.LocalEndPoint.ToString(), id );
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

        public Win32SocketAdapter( Socket connection, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
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

        public Win32SocketAdapter( IEndpointConfiguration endpoint, IServerConfiguration configuration )
        {
            try 
            {
                //Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
                Configuration = configuration;
                Connection = Bind( endpoint );
                Bytes = new byte[configuration.ReadBufferSize];
                SocketStream = new NetworkStream( Connection );
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

    [StructLayout(LayoutKind.Sequential)]
    public class WSAData 
    {
        public Int16 wVersion;
        public Int16 wHighVersion;  
        public String szDescription;  
        public String szSystemStatus;  
        public Int16 iMaxSockets;  
        public Int16 iMaxUdpDg;  
        public IntPtr lpVendorInfo;
    }

    [ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
    public class WSAQUERYSET
    {
        public Int32 dwSize = 0;  
        public String szServiceInstanceName = null;  
        public IntPtr lpServiceClassId;  
        public IntPtr lpVersion;  
        public String lpszComment;  
        public Int32 dwNameSpace;  
        public IntPtr lpNSProviderId;  
        public String lpszContext;  
        public Int32 dwNumberOfProtocols;  
        public IntPtr lpafpProtocols;  
        public String lpszQueryString;  
        public Int32 dwNumberOfCsAddrs;  
        public IntPtr lpcsaBuffer;  
        public Int32 dwOutputFlags;  
        public IntPtr lpBlob;
    }

    [StructLayout(LayoutKind.Sequential, Size=16)]
    public struct sockaddr_in
    {
        public const int Size = 16;

        public short sin_family;
        public ushort sin_port;
        public struct in_addr
        {
            public uint S_addr;
            public struct _S_un_b
            {
                public byte s_b1, s_b2, s_b3, s_b4;
            }
            public _S_un_b S_un_b;
            public struct _S_un_w
            {
                public ushort s_w1, s_w2;
            }
            public _S_un_w S_un_w;
        }
        public in_addr sin_addr;
    }

    public unsafe struct SOCKET
    {
        private void* handle;
        private SOCKET(int _handle)
        {
            handle = (void*)_handle;
        }
        public static bool operator ==(SOCKET s, int i)
        {
            return ((int)s.handle == i);
        }
        public static bool operator !=(SOCKET s, int i)
        {
            return ((int)s.handle != i);
        }
        public static implicit operator SOCKET(int i)
        {
            return new SOCKET(i);
        }
        public static implicit operator uint(SOCKET s)
        {
            return (uint)s.handle;
        }
        public override bool Equals(object obj)
        {
            return (obj is SOCKET) ? (((SOCKET)obj).handle == this.handle) : base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return (int)handle;
        }
    }

    public enum AddressFamily : int
    {
        Unknown = 0,
        InterNetworkv4 = 2,
        Ipx = 4,
        AppleTalk = 17,
        NetBios = 17,
        InterNetworkv6 = 23,
        Irda = 26,
        BlueTooth = 32
    }

    public enum SocketType : int
    {
        Unknown = 0,
        Stream = 1,
        DGram = 2,
        Raw = 3,
        Rdm = 4,
        SeqPacket = 5
    }
    public enum ProtocolType : int
    {
        BlueTooth = 3,
        Tcp = 6,
        Udp = 17,
        ReliableMulticast = 113
    }

    // fd_set used in 'select' method
    public unsafe struct fd_set
    {
        public const int FD_SETSIZE = 64;
        public uint fd_count;
        public fixed uint fd_array[FD_SETSIZE];
    }

    public unsafe partial class Native
    {
        public const int SOCKET_ERROR = -1;
        public const int INVALID_SOCKET = ~0;

        [DllImport("Ws2_32.dll")]
        public static extern int WSAStartup(ushort Version, out WSAData Data);
        [DllImport("Ws2_32.dll")]
        public static extern SocketError WSAGetLastError();
        [DllImport("Ws2_32.dll")]
        public static extern SOCKET socket(AddressFamily af, SocketType type, ProtocolType protocol);
        [DllImport("Ws2_32.dll")]
        public static extern int send(SOCKET s, byte* buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static extern int recv(SOCKET s, byte* buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static extern SOCKET accept(SOCKET s, void* addr, int addrsize);
        [DllImport("Ws2_32.dll")]
        public static extern int listen(SOCKET s, int backlog);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Ansi)]
        public static extern uint inet_addr(string cp);
        [DllImport("Ws2_32.dll")]
        public static extern ushort htons(ushort hostshort);
        [DllImport("Ws2_32.dll")]
        public static extern int connect(SOCKET s, sockaddr_in* addr, int addrsize);
        [DllImport("Ws2_32.dll")]
        public static extern int closesocket(SOCKET s);
        [DllImport("Ws2_32.dll")]
        public static extern int getpeername(SOCKET s, sockaddr_in* addr, int* addrsize);
        [DllImport("Ws2_32.dll")]
        public static extern int bind(SOCKET s, sockaddr_in* addr, int addrsize);
        
        //[DllImport("Ws2_32.dll")]
        //public static extern int select(int ndfs, fd_set* readfds, fd_set* writefds, fd_set* exceptfds, timeval* timeout);

        [DllImport("Ws2_32.dll")]
        public static extern sbyte* inet_ntoa(sockaddr_in.in_addr _in);
    }
}