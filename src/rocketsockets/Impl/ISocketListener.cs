using System;

namespace rocketsockets
{
    public interface ISocketListener
        : IDisposable
    {
        /// <summary>
        /// Closes the socket, attempting to send immediately send a TCP FIN (not RST)
        /// </summary>
        void Close();

        /// <summary>
        /// Begins listening to the socket and invokes the provided call-back on successful client connections.
        /// </summary>
        /// <param name="onSocket">The callback to invoke on client connect</param>
        void ListenTo( Action<ISocket> onSocket );
    }
}