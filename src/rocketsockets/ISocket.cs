// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System;

namespace rocketsockets
{
    /// <summary>
    /// An abstraction around different representation of sockets.
    /// Not intended for direct usage, prefer ISocketHandle instead
    /// for most uses.
    /// </summary>
    public interface ISocket
        : IDisposable
    {
        /// <summary>
        /// An arbitrary, unique identifier for the socket instance. Helpful when identifying the source of incoming bytes.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A way to add simple actions which will be notified in the event this socket instance closes.
        /// </summary>
        /// <param name="onClose">The closure to call</param>
        void AddCloseCallback( Action onClose );

        /// <summary>
        /// Closes the socket, attempting to send immediately send a TCP FIN (not RST)
        /// </summary>
        void Close();

        /// <summary>
        /// Asynchronously read the socket and invoke the appropriate callback.
        /// </summary>
        /// <param name="onBytes">A callback to process bytes received</param>
        /// <param name="onException">A callback to handle exceptions</param>
        void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException );

        /// <summary>
        /// Asynchronously write to the socket and notify caller when operation is complete.
        /// </summary>
        /// <param name="segment">The bytes to write to the socket</param>
        /// <param name="onComplete">A callback invoked when the write has completed</param>
        /// <param name="onException">A callback to handle exceptions</param>
        void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
    }
}