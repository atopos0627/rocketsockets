using System;

namespace rocketsockets.test
{
    public class SocketStub : ISocket
    {
        public Action OnClose { get; set; }
        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action OnWriteComplete { get; set; }
        public Action<Exception> OnWriteException { get; set; }

        public void Dispose()
        {
            OnClose();
            OnClose = null;
            OnBytes = null;
            OnException = null;
            OnWriteComplete = null;
            OnWriteException = null;
        }

        public string Id { get; set; }

        public void AddCloseCallback( Action onClose )
        {
            OnClose = onClose;
        }

        public void Close()
        {
            Dispose();
        }

        public void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException )
        {
            OnBytes = onBytes;
            OnException = onException;
        }

        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            OnWriteComplete = onComplete;
            OnWriteException = onException;
        }
    }
}