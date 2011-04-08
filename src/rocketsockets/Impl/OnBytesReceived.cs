using System;

namespace rocketsockets
{
    public delegate void OnBytesReceived( string receivedFrom, ArraySegment<byte> segment );

    public delegate Action OnConnectionReceived( ISocketHandle socket );
}