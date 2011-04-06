using System;
using System.Threading.Tasks;

namespace rocketsockets
{
    public class SocketEventLoop :
        ISocketLoop
    {
        public SocketNode Root { get; set; }
        public bool Running { get; set; }

        public ISocketHandle AddSocket( string id, ISocket socket, OnBytesReceived onBytes ) 
        {
            socket.AddCloseCallback( () => RemoveSocket( id ) );
            var node = new SocketNode( id, socket, onBytes );
            Root.AddNode( node );
            return node;
        }

        public void Loop()
        {
            var node = Root;
            while( Running )
            {
                node = node ?? Root;
                if( node.Available )
                {
                    node.ExecuteNextWrite();
                    node.ExecuteNextRead();
                }
                node = node.Next;
            }
        }

        public void RemoveSocket( ISocket socket )
        {
            var node = Root.Next;
            while( node != Root ) 
            {
                if( node == socket )
                {
                    node.Remove();
                    break;
                }
                node = node.Next;
            }
        }

        public void RemoveSocket( string id )
        {
            var node = Root.Next;
            while( node != Root ) 
            {
                if( node.Id == id )
                {
                    node.Remove();
                    break;
                }
                node = node.Next;
            }
        }

        public void Start() 
        {
            Running = true;
            var task = Task.Factory.StartNew( Loop );
        }

        public void Stop() 
        {
            Running = false;
        }

        public SocketEventLoop( ) 
        {
            Root = new SocketNode( "", null, null );
            Root.Next = Root;
        }
    }
}