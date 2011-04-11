using System;
using System.Runtime.InteropServices;

namespace rocketsockets
{
    [ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
    public unsafe struct WSAPROTOCOLCHAIN
    {
        public int ChainLen;
        public Int32 ChainEntries;
    }
}