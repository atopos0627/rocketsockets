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
using Symbiote.Core;

namespace rocketsockets
{
    public static class RocketServer
    {
        /// <summary>
        /// Provide configuration for the socket server. This will NOT start the server.
        /// Please take a dependency on ISocketServer and call the Start/Stop methods to
        /// control the socket server.
        /// </summary>
        public static IAssimilate RocketSockets( this IAssimilate assimilate, Action<IConfigurator> configurate )
        {
            var configurator = new ServerConfigurator();
            configurate( configurator );
            assimilate.Dependencies( x => x.For<IServerConfiguration>().Use( configurator.Configuration ) );
            return assimilate;
        }

        /// <summary>
        /// Provide configuration for the socket server. This will NOT start the server.
        /// Please take a dependency on ISocketServer and call the Start/Stop methods to
        /// control the socket server.
        /// </summary>
        public static void Configure( Action<IConfigurator> configurate )
        {
            Assimilate
                .Initialize()
                .RocketSockets( configurate );
        }
    }
}
