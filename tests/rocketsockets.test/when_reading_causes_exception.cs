using System;
using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_reading_causes_exception : with_socket_handle
    {
        private Because of = () => { 
                                       handle.Read();
                                       socket.OnException( new Exception("this is a test"));
        };

        private It should_close_underlying_socket = () => underlyingSocketClosed.ShouldBeTrue();
    }
}