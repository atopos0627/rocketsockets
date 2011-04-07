using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace rocketsockets
{
    public class SocketEventLoop :
        ISocketLoop
    {
        public bool Running { get; set; }
        public ConcurrentQueue<Action> ActionQueue { get; set; }
        
        public void Loop()
        {
            while( Running )
            {
                Action action = null;
                if( ActionQueue.TryDequeue( out action ) )
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        Thread.Yield();
                    }
                }
                else 
                {
                    Thread.Sleep( 1 );
                }
            }
        }

        public void Enqueue( Action action ) 
        {
            ActionQueue.Enqueue( action );
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
            ActionQueue = new ConcurrentQueue<Action>();
        }
    }
}