using Machine.Specifications;

namespace rocketsockets.test
{
    public class when_using_default_endpoint : with_default_server_setup
    {
        private It should_have_one_endpoint = () => testConfiguration.Endpoints.Count.ShouldEqual( 1 );
        private It should_be_named_default = () => testConfiguration.Endpoints[0].Name.ShouldEqual( "default" );
        private It should_bind_to_all = () => testConfiguration.Endpoints[0].AnyInterface.ShouldBeTrue();
        private It should_use_port_8998 = () => testConfiguration.Endpoints[0].Port.ShouldEqual( 8998 );
        private It should_not_use_ssl = () => testConfiguration.Endpoints[0].SSL.ShouldBeFalse();
    }
}