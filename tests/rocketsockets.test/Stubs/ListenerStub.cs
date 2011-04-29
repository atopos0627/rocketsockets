using System;
using rocketsockets.Impl;

namespace rocketsockets.test.Stubs
{
    public class ListenerStub : ISocketListener
    {
        public bool Closed { get; set; }
        public bool Disposed { get; set; }
        public bool Listening { get; set; }
        public Action<ISocket> OnSocket { get; set; } 

        public void Dispose()
        {
            Disposed = true;
        }

        public void Close()
        {
            Closed = true;
        }

        public void ListenTo( Action<ISocket> onSocket )
        {
            OnSocket = onSocket;
            Listening = true;
        }
    }
}
