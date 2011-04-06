using System;

namespace rocketsockets
{
    public interface ISocketServer
    {
        bool Running { get; }
        void Start( OnConnectionReceived onConnection, OnBytesReceived onBytes );
        void Stop();
    }
}