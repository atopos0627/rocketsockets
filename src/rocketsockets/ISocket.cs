using System;

namespace rocketsockets
{
    public interface ISocket
    {
        string Id { get; }
        void AddCloseCallback( Action onClose );
        void ListenTo( Action<ISocket> onSocket );
        void Close();
        void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException );
        void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
    }
}