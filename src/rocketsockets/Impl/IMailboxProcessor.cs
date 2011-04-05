using System;

namespace rocketsockets
{
    public interface IMailboxProcessor
    {
        void Remove( string id );
        void Start();
        void Stop();
        void Write( string Id, ArraySegment<byte> bytes );
    }
}