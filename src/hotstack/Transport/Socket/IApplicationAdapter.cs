using System;
using rocketsockets;

namespace hotstack.Transport.Socket 
{
    public interface IApplicationAdapter
    {
        Action AddSocket( string id, ISocketHandle socket );
        void HandleNextRead( string id, ArraySegment<byte> bytes );
    }
}
