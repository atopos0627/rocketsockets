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

namespace rocketsockets
{
    public interface IConfigureEndpoint
    {
        /// <summary>
        /// Bind this endpoint to all available network interfaces.
        /// </summary>
        IConfigureEndpoint BindToAll();
        
        /// <summary>
        /// Bind to a specific IP address or host name.
        /// </summary>
        /// <param name="endpoint">IP address or host name for this endpoint</param>
        IConfigureEndpoint BindTo( string endpoint );
        
        /// <summary>
        /// The port number to bind the endpoint to.
        /// </summary>
        /// <param name="port">Port number to listen on.</param>
        IConfigureEndpoint Port( int port );
        
        /// <summary>
        /// !!!THIS IS NOT CURRENTLY IMPLEMENTED!!!
        /// </summary>
        IConfigureEndpoint SecureSockets();
    }
}