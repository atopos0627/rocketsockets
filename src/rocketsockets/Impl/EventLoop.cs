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
        public ManualResetEventSlim Wait { get; set; }
        public CancellationToken Cancel { get; set; }
        
        public void Loop()
        {
            Action action = null;
            while( Running )
            {
                if( ActionQueue.TryDequeue( out action ) )
                {
                    try
                    {
                        action();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex );
                    }
                }
                else 
                {
                    //Thread.Sleep( 0 );
                    Wait.Reset();
                    Wait.Wait( Cancel );
                }
            }
        }

        public void Enqueue( Action action ) 
        {
            ActionQueue.Enqueue( action );
            Wait.Set();
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
            Cancel.WaitHandle.Close();
        }

        public EventLoop( ) 
        {
            ActionQueue = new ConcurrentQueue<Action>();
            Wait = new ManualResetEventSlim( false, 10 );
            Cancel = new CancellationToken();
        }
    }
}