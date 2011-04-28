using System;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_reading_from_socket_handle : with_socket_handle
    {
        private Because of = () => { 
                                       handle.Read();
                                       socket.OnBytes( new ArraySegment<byte>( new byte[] { }));
        };

        private It should_have_read_bytes = () => bytesRead.ShouldBeTrue();
    }
}