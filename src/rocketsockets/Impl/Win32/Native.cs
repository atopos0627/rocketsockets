﻿using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace rocketsockets
{
    public unsafe partial class Native
    {
        public const int SOCKET_ERROR = -1;
        public const int INVALID_SOCKET = ~0;

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int WSAStartup(ushort Version, out WSAData Data);

        [DllImport("ws2_32.dll",CharSet = CharSet.Auto, SetLastError=true)]
        public static extern Int32 WSACleanup();

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern SocketError WSAGetLastError();
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern SOCKET socket(AddressFamily af, SocketType type, ProtocolType protocol);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int send(SOCKET s, byte* buf, int len, int flags);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int recv(SOCKET s, byte* buf, int len, int flags);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern SOCKET accept(SOCKET s, void* addr, int addrsize);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int listen(SOCKET s, int backlog);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern uint inet_addr(string cp);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern ushort htons(ushort hostshort);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int connect(SOCKET s, sockaddr_in* addr, int addrsize);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int closesocket(SOCKET s);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int getpeername(SOCKET s, sockaddr_in* addr, int* addrsize);
        
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int bind(SOCKET s, sockaddr_in* addr, int addrsize);
        
        //[DllImport("Ws2_32.dll")]
        //public static extern int select(int ndfs, fd_set* readfds, fd_set* writefds, fd_set* exceptfds, timeval* timeout);

        [DllImport("Ws2_32.dll")]
        public static extern sbyte* inet_ntoa(sockaddr_in.in_addr _in);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int setsockopt(SOCKET s, int level, int optname, char optval, int optlen );

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int WSASocket( int addressFamily, int socketType, int protocol, out WSAPROTOCOL_INFO info, int group, int flags );

    }
}