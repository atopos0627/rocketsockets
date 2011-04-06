using System;
using hotstack.Config;
using Symbiote.Core;

namespace hotstack
{
    public static class HotstackServer
    {
        public static IAssimilate Hotstack( this IAssimilate assimilate, Action<IConfigureHttp> configurate )
        {
            var configurator = Assimilate.GetInstanceOf<IConfigureHttp>() as HttpConfigurator;
            configurate( configurator );
            assimilate.Dependencies( 
                x => x.For<HttpServerConfiguration>()
                .Use( configurator.ServerConfigurator.Configuration ) 
            );
            return assimilate;
        }

        public static void Configure( Action<IConfigureHttp> configurate )
        {
            Assimilate
                .Initialize()
                .Hotstack(configurate);
        }
    }
}
