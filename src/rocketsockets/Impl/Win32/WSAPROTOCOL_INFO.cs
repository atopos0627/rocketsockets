using System;
using System.Runtime.InteropServices;

namespace rocketsockets
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