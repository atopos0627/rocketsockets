using System;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_write_fails : with_socket_handle
    {
        protected static bool writeCompleted;
        protected static bool exceptionOccurred;

        private Because of = () => { 
                                       handle.Write( new ArraySegment<byte>(), () => writeCompleted = true, x => exceptionOccurred = true );
                                       socket.OnWriteException( new Exception("test") );
        };

        private It should_show_write_complete = () => exceptionOccurred.ShouldBeTrue();
        private It should_not_call_write_complete = () => writeCompleted.ShouldBeFalse();
    }
}