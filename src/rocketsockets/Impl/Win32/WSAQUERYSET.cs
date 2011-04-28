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
using System.Runtime.InteropServices;

namespace rocketsockets.Impl.Win32
{
    [ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
    public class WSAQUERYSET
    {
        public Int32 dwSize = 0;  
        public String szServiceInstanceName = null;  
        public IntPtr lpServiceClassId;  
        public IntPtr lpVersion;  
        public String lpszComment;  
        public Int32 dwNameSpace;  
        public IntPtr lpNSProviderId;  
        public String lpszContext;  
        public Int32 dwNumberOfProtocols;  
        public IntPtr lpafpProtocols;  
        public String lpszQueryString;  
        public Int32 dwNumberOfCsAddrs;  
        public IntPtr lpcsaBuffer;  
        public Int32 dwOutputFlags;  
        public IntPtr lpBlob;
    }
}