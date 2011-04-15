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

using System.Runtime.InteropServices;

namespace rocketsockets.Impl.Win32
{
    [StructLayout(LayoutKind.Sequential, Size=16)]
    public struct sockaddr_in
    {
        public const int Size = 16;

        public short sin_family;
        public ushort sin_port;

        public struct in_addr
        {
            public uint S_addr;
            
            public struct _S_un_b
            {
                public byte s_b1, s_b2, s_b3, s_b4;
            }
            public _S_un_b S_un_b;

            public struct _S_un_w
            {
                public ushort s_w1, s_w2;
            }
            public _S_un_w S_un_w;
        }

        public in_addr sin_addr;
    }
}