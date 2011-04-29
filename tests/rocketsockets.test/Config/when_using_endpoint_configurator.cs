using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_using_endpoint_configurator
        : with_endpoint_configurator
    {
        private It should_have_correct_name = () => testConfiguration.Name.ShouldEqual( "temp" );
        private It should_have_correct_ip = () => testConfiguration.BindTo.ShouldEqual( "127.0.0.1" );
        private It should_have_correct_port = () => testConfiguration.Port.ShouldEqual( 10981 );
        private It should_bind_to_any = () => testConfiguration.AnyInterface.ShouldBeTrue();
    }
}