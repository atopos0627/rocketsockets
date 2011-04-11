using System.Runtime.InteropServices;

namespace rocketsockets
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