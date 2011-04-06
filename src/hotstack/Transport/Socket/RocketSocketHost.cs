using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hotstack.Owin;
using hotstack.Owin.Impl;
using rocketsockets;

namespace hotstack.Transport.Socket
{
    public class RocketSocketHost :
        IOwinHost
    {
        public ISocketServer Server { get; set; }
        public IApplicationAdapter Adapter { get; set; }

        public void Start()
        {
            Server.Start( Adapter.AddSocket, Adapter.HandleNextRead );
        }

        public void Stop()
        {
            Server.Stop();
        }

        public RocketSocketHost( ISocketServer server, IApplicationAdapter adapter )
        {
            Server = server;
            Adapter = adapter;
        }
    }
}
