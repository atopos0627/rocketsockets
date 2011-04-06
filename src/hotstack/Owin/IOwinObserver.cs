using System;

namespace hotstack.Owin
{
    public interface IOwinObserver
    {
        bool OnNext( ArraySegment<byte> segment, Action continuation );
        void OnError( Exception exception );
        void OnComplete();
    }
}