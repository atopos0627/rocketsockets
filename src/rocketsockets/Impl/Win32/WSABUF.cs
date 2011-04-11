using System.Runtime.InteropServices;

namespace rocketsockets
{
    [ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
    public unsafe struct WSABUF
    {
        public ulong Length;
        public char[] Buffer;
    }
}