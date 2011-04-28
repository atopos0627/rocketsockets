using System;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_write_succeeds : with_socket_handle
    {
        protected static bool writeCompleted;

        private Because of = () => { 
                                       handle.Write( new ArraySegment<byte>(), () => writeCompleted = true, x => { } );
                                       socket.OnWriteComplete();
        };

        private It should_show_write_complete = () => writeCompleted.ShouldBeTrue();
    }
}