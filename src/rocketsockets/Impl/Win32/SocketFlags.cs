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

namespace rocketsockets.Impl.Win32
{
    public enum SocketFlags : int
    {
        Overlapped = 0x01,
        Multipoint_C_Root = 0x02,
        Multipoint_C_Leaf = 0x04,
        Multipoint_D_Root = 0x08,
        Multipoint_D_Leaf = 0x10,
        Access_System_Security = 0x40,
        No_Handle_Inherit = 0x80
    }
}