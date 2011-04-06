﻿using System;

namespace rocketsockets
{
    public interface ISocketLoop
    {
        ISocketHandle AddSocket( string id, ISocket socket, OnBytesReceived onBytes );
        void RemoveSocket( ISocket socket );
        void Start();
        void Stop();
    }
}