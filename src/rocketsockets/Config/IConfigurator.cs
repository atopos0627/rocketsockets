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

namespace rocketsockets.Config
{
    public interface IConfigurator
    {
        /// <summary>
        /// Add an additional endpoint for the server to listen to and provide the configuration
        /// for that endpoint.
        /// </summary>
        IConfigurator AddEndpoint( string name, Action<IConfigureEndpoint> configurator );
        
        /// <summary>
        /// Specifies an certificate for use on the server to create a secure socket connection
        /// </summary>
        IConfigurator SecureWithCertFrom( string certPath );

        /// <summary>
        /// The default includes all network interfaces on port 8998.
        /// </summary>
        IConfigurator UseDefaultEndpoint();
    }
}