using System;

namespace rocketsockets
{
    public class ServerConfigurator
        : IConfigurator
    {
        public ServerConfiguration Configuration { get; set; }

        public IConfigurator AddEndpoint( string name, Action<IConfigureEndpoint> configurator )
        {
            var endpoint = new EndpointConfigurator( name );
            configurator( endpoint );
            Configuration.AddEndPoint( endpoint.Configuration );
            return this;
        }

        public IConfigurator UseDefaultEndpoint()
        {
            var endpoint = new EndpointConfiguration( "default" )
            {
                BindTo = new [] { "0.0.0.0" },
                Port = 8998,
                SSL = false
            };
            Configuration.AddEndPoint( endpoint );
            return this;
        }

        public ServerConfigurator() 
        {
            Configuration = new ServerConfiguration()
            {
                ReadBufferSize = 4 * 1024,
                WriteBufferSize = 4 * 1024,
            };
        }
    }
}