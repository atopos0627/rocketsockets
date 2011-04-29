using System;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class EventLoopStub : IEventLoop
    {
        public bool Running { get; set; }

        public void Enqueue( Action action )
        {
            action();
        }

        public void Start( int workers )
        {
            
        }

        public void Stop()
        {
            
        }
    }
}