using System;

namespace rocketsockets
{
    public interface IMailbox
    {
        void Write( ArraySegment<byte> message );
        void Process( Action<ArraySegment<byte>> action );
    }
}