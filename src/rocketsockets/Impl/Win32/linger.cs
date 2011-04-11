using System.Runtime.InteropServices;

namespace rocketsockets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct linger
    {
        public ushort onoff;
        public ushort lingerFor;
    }
}