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

namespace hotstack.Config
{
    public class HttpServerConfigurator :
        IConfigureServerSettings
    {
        public HttpServerConfiguration Configuration { get; set; }

        public IConfigureServerSettings BasePath( string path )
        {
            Configuration.BaseContentPath = path;
            return this;
        }

        public IConfigureServerSettings DefaultLayoutTemplate( string layoutName )
        {
            Configuration.DefaultLayoutTemplate = layoutName;
            return this;
        }

        public IConfigureServerSettings AddViewSearchFolder( string path )
        {
            Configuration.PathSources.Add( path );
            return this;
        }

        public HttpServerConfigurator()
        {
            Configuration = new HttpServerConfiguration();
        }
    }
}