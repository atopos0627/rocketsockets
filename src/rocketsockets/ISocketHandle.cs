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
    /// A very simple abstraction around connected sockets that
    /// provides asynchronous operations performed on an event loop
    /// </summary>
    public interface ISocketHandle
    {
        /// <summary>
        /// An arbitrary, unique identifier for the socket instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Closes the underlying socket connection
        /// </summary>
        void Close();

        /// <summary>
        /// Enqueues an asynchronous read operation on the event loop for this socket
        /// </summary>
        void Read();
        
        /// <summary>
        /// Enqueues an asynchronous write operation on the event loop for this socket
        /// </summary>
        /// <param name="segment">The bytes to write to the socket</param>
        /// <param name="onComplete">The callback to invoke on write completion</param>
        /// <param name="onException">The callback to handle exceptions</param>
        void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
    }
}