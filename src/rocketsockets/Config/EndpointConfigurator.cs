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
    public class EndpointConfigurator :
        IConfigureEndpoint
    {
        public IEndpointConfiguration Configuration { get; set; }

        public IConfigureEndpoint BindToAll()
        {
            Configuration.AnyInterface = true;
            return this;
        }

        public IConfigureEndpoint BindTo( string endpoint )
        {
            Configuration.AnyInterface = false;
            Configuration.BindTo = endpoint;
            return this;
        }

        public IConfigureEndpoint SecureSockets()
        {
            Configuration.SSL = true;
            return this;
        }

        public IConfigureEndpoint Port( int port )
        {
            Configuration.Port = port;
            return this;
        }

        public EndpointConfigurator( string name )
        {
            Configuration = new EndpointConfiguration( name );
        }
    }
}