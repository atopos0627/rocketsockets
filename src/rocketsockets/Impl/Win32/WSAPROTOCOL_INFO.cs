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
using System.Runtime.InteropServices;

namespace rocketsockets.Impl.Win32
{
    [ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
    public unsafe struct WSAPROTOCOL_INFO
    {
        public Int32 ServiceFlags1;
        public Int32 ServiceFlags2;
        public Int32 ServiceFlags3;
        public Int32 ServiceFlags4;
        public Int32 ServiceFlags5;
        public Guid ProviderId;
        public Int32 CatalogEntryId;
        public WSAPROTOCOLCHAIN ProtocolChain;
        public int Version;
        public int AddressFamily;
        public int MaxSocketAddress;
        public int MinSocketAddress;
        public int SocketType;
        public int Protocol;
        public int ProtocolMaxOffset;
        public int NetworkByteOrder;
        public int SecurityScheme;
        public Int32 MessageSize;
        public Int32 ProviderReserved;
        public string ProtocolName;
    }
}