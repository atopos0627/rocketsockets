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

namespace rocketsockets
{
    public unsafe struct SOCKET
    {
        private void* handle;

        private SOCKET(int _handle)
        {
            handle = (void*)_handle;
        }

        public SOCKET(IntPtr _handle)
        {
            handle = _handle.ToPointer();
        }

        public static bool operator ==(SOCKET s, int i)
        {
            return ((int)s.handle == i);
        }
        public static bool operator !=(SOCKET s, int i)
        {
            return ((int)s.handle != i);
        }
        
        public static implicit operator SOCKET(int i)
        {
            return new SOCKET(i);
        }

        public static implicit operator uint(SOCKET s)
        {
            return (uint)s.handle;
        }
        public override bool Equals(object obj)
        {
            return (obj is SOCKET) ? (((SOCKET)obj).handle == this.handle) : base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return (int)handle;
        }
    }
}