using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace rocketsockets
{
    public class EventLoop :
        IEventLoop
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

        public void Start( int workers ) 
        {
            Running = true;
            for( int i = 0; i < workers; i ++ )
                Task.Factory.StartNew( Loop );
        }

        public void Stop() 
        {
            Running = false;
        }

        public EventLoop( ) 
        {
            ActionQueue = new ConcurrentQueue<Action>();
        }
    }
}