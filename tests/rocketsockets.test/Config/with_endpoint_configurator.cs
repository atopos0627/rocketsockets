using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Machine.Specifications;
using rocketsockets.Config;

namespace rocketsockets.test
{
    public class with_endpoint_configurator
    {
        protected static IEndpointConfiguration testConfiguration;
        protected static IConfigureEndpoint configurator;

        private Establish context = () => { 
            configurator = new EndpointConfigurator( "temp" );
            configurator.BindTo("127.0.0.1").BindToAll().Port( 10981 ).SecureSockets();
            testConfiguration = ( configurator as EndpointConfigurator ).Configuration;
        };
    }
}
