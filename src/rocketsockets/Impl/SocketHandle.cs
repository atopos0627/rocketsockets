using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketHandle :
        ISocketHandle
    {
        public string Id { get { return Connection.Id; } }
        public ISocket Connection { get; set; }
        public IEventLoop ReadLoop { get; set; }
        public IEventLoop WriteLoop { get; set; }
        public IEventLoop DisposeLoop { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        public void Close() 
        {
            WriteLoop = null;
            ReadLoop = null;
            OnBytes = null;
            ReadCount = 0;
            WriteCount = 0;
            //Connection.Close();
            //Connection = null;
            //DisposeLoop = null;
            DisposeLoop.Enqueue( () =>
            {
                Connection.Close();
                Connection = null;
                DisposeLoop = null;
            } );
        }

        public void HandleReadException( Exception exception ) 
        {

        }

        public void Read()
        {
            ReadLoop.Enqueue( () => 
                Connection.Read( 
                    x => OnBytes( Id, x ), 
                    HandleReadException ) 
                );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            WriteLoop.Enqueue( () => 
                Connection.Write(
                            segment,
                            onComplete,
                            onException )
                );
        }
		
        public SocketHandle( ISocket socket, IEventLoop readLoop, IEventLoop writeLoop, IEventLoop disposeLoop, OnBytesReceived onBytes )
        {
            OnBytes = onBytes;
            Connection = socket;
            ReadLoop = readLoop;
            WriteLoop = writeLoop;
            DisposeLoop = disposeLoop;
        }
    }
}