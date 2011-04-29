using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_starting_server : with_server
    {
        private Because of = () => server.Start( h => () => { }, (id, b) => { } );
            
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
    }
}