using System.Collections.Generic;

namespace rocketsockets
{
    public interface IServerConfiguration
    {
        IList<IEndpointConfiguration> Endpoints { get; }
        int ReadBufferSize { get; set; }
        int WriteBufferSize { get; set; }
    }
}