using System;
using rocketsockets.Impl;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class SchedulerStub : IScheduler
    {
        public IEventLoop Loop { get; set; }
        public bool Started { get; set; }
        public bool Stopped { get; set; }
        public bool Disposed { get; set; }

        public void Dispose()
        {
            Disposed = true;
        }

        public void QueueOperation( Operation type, Action operation )
        {
            Loop.Enqueue( operation );
        }

        public void Start()
        {
            Started = true;
        }

        public void Stop()
        {
            Stopped = true;
        }

        public SchedulerStub() 
        {
            Loop = new EventLoopStub();
        }
    }
}