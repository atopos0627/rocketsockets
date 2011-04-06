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
using hotstack.Owin;
using hotstack.Transport.Socket;
using Symbiote.Core;

namespace hotstack.Config
{
    public class HttpConfigurator :
        IConfigureHttp
    {
        public IRegisterApplication RegisterApplication { get; set; }
        public HttpServerConfigurator ServerConfigurator {get; set; }

        public IConfigureHttp ConfigureHost( Action<IConfigureServerSettings> configurator )
        {
            configurator( ServerConfigurator );
            return this;
        }

        public IConfigureHttp RegisterApplications( Action<IRegisterApplication> register )
        {
            register( RegisterApplication );
            return this;
        }

        public IConfigureHttp HostOnSockets()
        {
            Assimilate.Dependencies( x => x.For<IOwinHost>().Use<RocketSocketHost>());
            return this;
        }

        public HttpConfigurator( IRegisterApplication registerApplication )
        {
            RegisterApplication = registerApplication;
            ServerConfigurator = new HttpServerConfigurator();

            Assimilate.Dependencies( x => x.For<HttpServerConfiguration>().Use( ServerConfigurator.Configuration ));
        }
    }
}
