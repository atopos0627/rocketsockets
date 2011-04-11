using System;
using System.Runtime.InteropServices;

namespace rocketsockets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WSAData
    {
        public Int16 version;
        public Int16 highVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
        public String description;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        public String systemStatus;

        public Int16 maxSockets;
        public Int16 maxUdpDg;
        public IntPtr vendorInfo;
    }
}