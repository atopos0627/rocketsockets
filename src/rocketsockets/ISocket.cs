﻿using System;

namespace rocketsockets
{
    public interface ISocket
    {
        void Close();
        void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException );
        void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
    }
}