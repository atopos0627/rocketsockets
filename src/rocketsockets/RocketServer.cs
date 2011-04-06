using System;
using Symbiote.Core;

namespace rocketsockets
{
    public static class RocketServer
    {
        public static IAssimilate RocketSockets( this IAssimilate assimilate, Action<IConfigurator> configurate )
        {
            var configurator = new ServerConfigurator();
            configurate( configurator );
            assimilate.Dependencies( x => x.For<IServerConfiguration>().Use( configurator.Configuration ) );
            return assimilate;
        }

        public static void Configure( Action<IConfigurator> configurate )
        {
            Assimilate
                .Initialize()
                .RocketSockets( configurate );
        }
    }
}
