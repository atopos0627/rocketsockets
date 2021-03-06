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
using System.Collections.Generic;

namespace rocketsockets.Config
{
    public class ServerConfiguration :
        IConfigureServer,
        IServerConfiguration
    {
        public string CertPath { get; set; }
        public IList<IEndpointConfiguration> Endpoints { get; set; }
        public int ReadBufferSize { get; set; }
        public bool Secure { get; set; }
        public int WriteBufferSize { get; set; }

        public void AddEndPoint( IEndpointConfiguration endpoint )
        {
            Endpoints.Add( endpoint );
        }

        public ServerConfiguration()
        {
            Endpoints = new List<IEndpointConfiguration>();
        }
    }
}