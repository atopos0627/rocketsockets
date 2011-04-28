using Machine.Specifications;
using rocketsockets.Config;

namespace rocketsockets.test
{
    public class with_default_server_setup : with_server_configurator
    {
        private Establish context = () => { 
                                              configurator.UseDefaultEndpoint();
                                              testConfiguration = ( configurator as ServerConfigurator ).Configuration;
        };
    }
}