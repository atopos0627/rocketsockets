using Machine.Specifications;
using rocketsockets.Config;
using Symbiote.Core;

namespace rocketsockets.test
{
    public class with_listener_setup : with_server_configurator
    {
        private Establish context = () => { 
                                              Assimilate.Initialize();
                                              configurator.UseDefaultEndpoint();
                                              testConfiguration = ( configurator as ServerConfigurator ).Configuration;
        };
    }
}