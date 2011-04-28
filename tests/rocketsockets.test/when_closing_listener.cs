using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_closing_listener : with_managed_socket_listener
    {
        private Because of = () => listener.Close();

        private It should_be_running = () => listener.Listening.ShouldBeFalse();
    }
}