namespace rocketsockets
{
    public class EndpointConfigurator :
        IConfigureEndpoint
    {
        public IEndpointConfiguration Configuration { get; set; }

        public IConfigureEndpoint BindTo( params string[] endpoints )
        {
            Configuration.BindTo = endpoints;
            return this;
        }

        public IConfigureEndpoint SecureSockets()
        {
            Configuration.SSL = true;
            return this;
        }

        public IConfigureEndpoint Port( int port )
        {
            Configuration.Port = port;
            return this;
        }

        public EndpointConfigurator( string name )
        {
            Configuration = new EndpointConfiguration( name );
        }
    }
}