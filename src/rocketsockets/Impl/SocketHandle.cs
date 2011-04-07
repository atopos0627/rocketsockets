using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketHandle :
        ISocketHandle
    {
        public string Id { get; set; }
        public ISocket Connection { get; set; }
        public IEventLoop Loop { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        public void Close() 
        {
            Loop = null;
            OnBytes = null;
            ReadCount = 0;
            WriteCount = 0;
            Connection.Close();
            Connection = null;
        }

        public void HandleReadException( Exception exception ) 
        {

        }

        public void Read()
        {
            Loop.Enqueue( () => 
                Connection.Read( 
                    x => OnBytes( Id, x ), 
                    HandleReadException ) 
                );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            Loop.Enqueue( () => 
                Connection.Write(
                            segment,
                            onComplete,
                            onException )
                );
        }
		
        public SocketHandle( string id, ISocket socket, IEventLoop loop, OnBytesReceived onBytes )
        {
            Id = id;
            OnBytes = onBytes;
            Connection = socket;
            Loop = loop;
        }
    }
}