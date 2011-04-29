using System.Linq;
using System.Threading;
using Machine.Specifications;
using rocketsockets.Impl;

namespace rocketsockets.test
{
    public class when_scheduling_operations : with_event_scheduler
    {
        static bool wrote;
        static bool read;
        static bool disposed;
        static bool generic;
        static bool connected;

        private Because of = () => 
        { 
            scheduler.Start();

            Thread.Sleep( 1000 );

            scheduler.QueueOperation( Operation.Read, () => read = true );
            scheduler.QueueOperation( Operation.Write, () => wrote = true );
            scheduler.QueueOperation( Operation.Connect, () => connected = true );
            scheduler.QueueOperation( Operation.Generic, () => generic = true );
            scheduler.QueueOperation( Operation.Dispose, () => disposed = true );

            scheduler.Stop();
        };

        It should_have_processed_read = () => read.ShouldBeTrue();
        It should_have_processed_wrote = () => wrote.ShouldBeTrue();
        It should_have_processed_connected = () => connected.ShouldBeTrue();
        It should_have_processed_generic = () => generic.ShouldBeTrue();
        It should_have_processed_dispose = () => disposed.ShouldBeTrue();
        It should_have_stopped_all_loops = () => scheduler.Loops.All( x => !x.Value.Running );
    }
}