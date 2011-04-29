using System;
using System.Text;
using System.Threading;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_reading_and_writing_to_managed_socket : with_managed_socket_adapter
    {
        static bool read_received;
        static bool read_exception;
        static bool write_succeeded;
        static bool write_exception;

        private Because of = () => 
        {
            socket.Read( x => read_received = true, x => read_exception = true );
            socket.Write( new ArraySegment<byte>(Encoding.UTF8.GetBytes( "hi" )), () => write_succeeded=true, x => write_exception = true );
            Thread.Sleep( 100 );
        };
        
        private It should_have_marked_connection_as_received = () => connectionReceived.ShouldBeTrue();
        private It should_have_written_correctly = () => write_succeeded.ShouldBeTrue();
        private It should_not_have_write_exception = () => write_exception.ShouldBeFalse();
        private It should_have_read_correctly = () => read_received.ShouldBeTrue();
        private It should_not_have_read_exception = () => read_exception.ShouldBeFalse();
    }
}