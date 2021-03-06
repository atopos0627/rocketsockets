﻿// /* 
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
    public class ServerConfigurator
        : IConfigurator
    {
        public ServerConfiguration Configuration { get; set; }

        public IConfigurator AddEndpoint( string name, Action<IConfigureEndpoint> configurator )
        {
            var endpoint = new EndpointConfigurator( name );
            configurator( endpoint );
            Configuration.AddEndPoint( endpoint.Configuration );
            return this;
        }

        public IConfigurator SecureWithCertFrom( string certPath ) 
        {
            Configuration.Secure = true;
            Configuration.CertPath = certPath;
            return this;
        }

        public IConfigurator UseDefaultEndpoint()
        {
            var endpoint = new EndpointConfiguration( "default" )
            {
                AnyInterface = true,
                Port = 8998,
                SSL = false
            };
            Configuration.AddEndPoint( endpoint );
            return this;
        }

        public ServerConfigurator() 
        {
            Configuration = new ServerConfiguration()
            {
                ReadBufferSize = 4 * 1024,
                WriteBufferSize = 4 * 1024,
            };
        }
    }
}