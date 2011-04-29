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
    public class SocketHandle :
        ISocketHandle
    {
        public string Id { get { return Connection.Id; } }
        public IScheduler Scheduler { get; set; }
        public ISocket Connection { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        public void Close() 
        {
            OnBytes = null;
            ReadCount = 0;
            WriteCount = 0;
            Scheduler.QueueOperation(
                Operation.Dispose,
                () =>
                {
                    Connection.Close();
                    Connection = null;
                    Scheduler = null;
                } );
        }

        public void HandleReadException( Exception exception ) 
        {
            Close();
        }

        public void Read()
        {
            Scheduler.QueueOperation( 
                Operation.Read, 
                () => 
                    Connection.Read( 
                    x => OnBytes( Id, x ), 
                    HandleReadException ) 
                );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            Scheduler.QueueOperation( 
                Operation.Write, 
                () => Connection.Write(
                            segment,
                            onComplete,
                            onException )
                );
        }
		
        public SocketHandle( ISocket socket, IScheduler scheduler, OnBytesReceived onBytes )
        {
            OnBytes = onBytes;
            Connection = socket;
            Scheduler = scheduler;
        }
    }
}