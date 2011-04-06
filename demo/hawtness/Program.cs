using System;
using hotstack;
using hotstack.Owin.Http;
using Symbiote.Core;
using rocketsockets;
using Symbiote.Daemon;

namespace hawtness
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .RocketSockets( x => x.UseDefaultEndpoint() )
                .Hotstack( x =>
                               {
                                   x.RegisterApplications( h =>
                                                               {
                                                                   h.DefineApplication<HelloWorld>( r => true );
                                                               } );
                               } )
                .Daemon( x => x.Arguments( args ).Name( "hawtness" ) )
                .RunDaemon();
        }
    }

    public class App : IDaemon
    {
        public IOwinHost Host { get; set; }

        public void Start()
        {
            Host.Start();
        }

        public void Stop()
        {
            Host.Stop();
        }

        public App( IOwinHost host )
        {
            Host = host;
        }
    }

    public class HelloWorld : Application
    {
        public override void OnError( Exception exception )
        {
            
        }

        public override void CompleteResponse()
        {
            Response
                .AppendToBody( "Hellizzle Wizzizzorld! It's da HTTPizzleDizzle Fo' Rizzle, Fo' Shizzle!" )
                .Submit( HttpStatus.Ok );
        }
    }
}
