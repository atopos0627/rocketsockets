using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_stopping_server : with_server
    {
        private Because of = () =>
        { 
            server.Start( h => () => { }, ( id, b ) => { } ); 
            server.Stop();
        };
        
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
        private It should_have_stopped_listener = () => listener.Closed.ShouldBeTrue();
        private It should_have_stopped_scheduler = () => scheduler.Stopped.ShouldBeTrue();
    }
}