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