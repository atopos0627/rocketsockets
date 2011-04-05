using System.Collections.Generic;

namespace rocketsockets
{
    public class ServerConfiguration :
        IConfigureServer,
        IServerConfiguration
    {
        public IList<IEndpointConfiguration> Endpoints { get; set; }
        public int ReadBufferSize { get; set; }
        public int WriteBufferSize { get; set; }

        public void AddEndPoint( IEndpointConfiguration endpoint )
        {
            Endpoints.Add( endpoint );
        }

        public ServerConfiguration()
        {
            Endpoints = new List<IEndpointConfiguration>();
        }
    }
}