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
            var node = new SocketNode( id, socket, onBytes );
            Root.AddNode( node );
            return node;
        }

        public void Loop()
        {
            var node = Root;
            while( Running )
            {
                if( node.Available )
                    if( !node.ExecuteNextRead() )
                        node.ExecuteNextWrite();
                node = node.Next;
            }
        }

        public void RemoveSocket( ISocket socket )
        {
            var node = Root.Next;
            while( node != Root ) 
            {
                if( node == socket )
                    socket.Close();
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
            Root = new SocketNode( null, null, null ) { Available = false };
            Root.Next = Root;
        }
    }
}