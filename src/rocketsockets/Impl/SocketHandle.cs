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
using rocketsockets.Impl;
using Symbiote.Core.Concurrency;

namespace rocketsockets
{
    public class SocketHandle :
        ISocketHandle
    {
        public string Id { get { return Connection.Id; } }
        public ISocket Connection { get; set; }
        public IEventLoop ReadLoop { get; set; }
        public IEventLoop WriteLoop { get; set; }
        public IEventLoop DisposeLoop { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        public void Close() 
        {
            WriteLoop = null;
            ReadLoop = null;
            OnBytes = null;
            ReadCount = 0;
            WriteCount = 0;
            DisposeLoop.Enqueue( () =>
            {
                Connection.Close();
                Connection = null;
                DisposeLoop = null;
            } );
        }

        public void HandleReadException( Exception exception ) 
        {
            Close();
        }

        public void Read()
        {
            ReadLoop.Enqueue( () => 
                Connection.Read( 
                    x => OnBytes( Id, x ), 
                    HandleReadException ) 
                );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            WriteLoop.Enqueue( () => 
                Connection.Write(
                            segment,
                            onComplete,
                            onException )
                );
        }
		
        public SocketHandle( ISocket socket, IEventLoop readLoop, IEventLoop writeLoop, IEventLoop disposeLoop, OnBytesReceived onBytes )
        {
            OnBytes = onBytes;
            Connection = socket;
            ReadLoop = readLoop;
            WriteLoop = writeLoop;
            DisposeLoop = disposeLoop;
        }
    }
}