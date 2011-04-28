using Machine.Specifications;
using rocketsockets.Config;

namespace rocketsockets.test
{
    public class with_server_configurator
    {
        protected static IConfigurator configurator;
        protected static IServerConfiguration testConfiguration;

        private Establish context = () => { 
                                              configurator = new ServerConfigurator();
        };
    }
}