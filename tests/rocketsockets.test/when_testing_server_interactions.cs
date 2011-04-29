using System;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_testing_server_interactions : with_server
    {
        public static bool receivedHandle;
        public static bool receivedBytes;
        public static bool closeHandleCalled;

        private Because of = () =>
        { 
            server.Start( 
                h =>
                {
                    receivedHandle = true;
                    h.Read();
                    return () => { closeHandleCalled = true; };
                }, 
                ( id, b ) => 
                { 
                    receivedBytes = true; 
                } );

            listener.OnSocket( socket );
            socket.OnBytes( new ArraySegment<byte>() );

            server.Stop();
        };
        
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
        private It should_have_stopped_listener = () => listener.Closed.ShouldBeTrue();
        private It should_have_stopped_scheduler = () => scheduler.Stopped.ShouldBeTrue();
        private It should_have_received_a_handle = () => receivedHandle.ShouldBeTrue();
        private It should_have_received_byte = () => receivedBytes.ShouldBeTrue();
    }
}