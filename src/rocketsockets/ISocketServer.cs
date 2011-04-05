using System;

namespace rocketsockets
{
    public interface ISocketServer
    {
        bool Running { get; }
        void Start( Action<ISocket> onConnection, Action<ArraySegment<byte>> onData );
        void Stop();
    }
}