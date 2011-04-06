using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using rocketsockets;

namespace hotstack.Owin.Impl 
{
    public interface IHandleReads
    {
        void AddSocket( string id, ISocketHandle socket );
        void HandleNextRead( string id, ArraySegment<byte> bytes );
    }

    
}
