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
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Concurrency;
using Symbiote.Core.Extensions;

namespace rocketsockets.Impl
{
    public class EventLoopScheduler : IScheduler
    {
        public Dictionary<Operation, IEventLoop> Loops { get; set; }

        public void QueueOperation( Operation type, Action operation )
        {
            Loops[type].Enqueue( operation );
        }

        public void Start()
        {
            Loops.ForEach( x => x.Value.Start( 1 ) );
        }

        public void Stop()
        {
            Loops.ForEach( x => x.Value.Stop() );
        }

        public EventLoopScheduler() 
        {
            Loops = new Dictionary<Operation, IEventLoop>();
            Enum.GetValues( typeof( Operation ) )
                .OfType<Operation>()
                .ForEach( x => Loops.Add( x, new EventLoop() ) );
        }

        public void Dispose()
        {
            Stop();
            Loops.Clear();
        }
    }
}