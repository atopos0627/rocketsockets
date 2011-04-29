using Machine.Specifications;
using rocketsockets.Config;
using Symbiote.Core;

namespace rocketsockets.test
{
    public class when_configuring_standalone
    {
        static IServerConfiguration configuration;
        static IEndpointConfiguration endpoint;

        private Because of = () => 
        { 
            RocketServer.Configure( x => x.UseDefaultEndpoint() );
            configuration = Assimilate.GetInstanceOf<IServerConfiguration>();
            endpoint = configuration.Endpoints[0];
        };

        private It should_have_one_endpoint = () => configuration.Endpoints.Count.ShouldEqual( 1 );

        private It should_have_default_endpoint = () =>
        {
            endpoint.AnyInterface.ShouldBeTrue();
            endpoint.SSL.ShouldBeFalse();
            endpoint.Port.ShouldEqual( 8998 );
        };
    }
}