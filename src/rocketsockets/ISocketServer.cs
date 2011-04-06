using System;

namespace rocketsockets
{
    public interface ISocketServer
    {
        bool Running { get; }
        void Start( Action<string, ISocketHandle> onConnection, OnBytesReceived onBytes );
        void Stop();
    }
}