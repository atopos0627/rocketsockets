using System;

namespace rocketsockets
{
    public interface ISocketHandle
    {
        void Close();
        void Read();
        void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
    }
}