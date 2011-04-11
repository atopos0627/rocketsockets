using System;
using System.Runtime.InteropServices;

namespace rocketsockets
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