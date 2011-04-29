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

namespace rocketsockets.Impl
{
    public interface ISocketListener
        : IDisposable
    {
        /// <summary>
        /// Closes the socket, attempting to send immediately send a TCP FIN (not RST)
        /// </summary>
        void Close();

        /// <summary>
        /// Indicates if the current listener is actively listening to the endpoint
        /// </summary>
        bool Listening { get; }

        /// <summary>
        /// Begins listening to the socket and invokes the provided call-back on successful client connections.
        /// </summary>
        /// <param name="onSocket">The callback to invoke on client connect</param>
        void ListenTo( Action<ISocket> onSocket );
    }
}