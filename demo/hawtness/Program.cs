using System;
using hotstack;
using hotstack.Owin.Http;
using Symbiote.Core;
using rocketsockets;
using Symbiote.Daemon;
using System.Linq;
using Symbiote.Log4Net;

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
                                                                   h.DefineApplication<Gtfo>( r => r.RequestUri.EndsWith( ".ico" ) );
                                                                   h.DefineApplication<HelloWorld>( r => true );
                                                               } );
                               } )
                //.AddConsoleLogger<ISocketServer>( x => x.Debug().MessageLayout( p => p.Message().Newline() ) )
                //.AddColorConsoleLogger<ISocketHandle>( x => x.Debug().MessageLayout( p => p.Message().Newline() ).DefineColor().BackGround.IsGreen().Text.IsBlack() )
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

    public class Gtfo : Application
    {
        public override void OnError( Exception exception )
        {
            Console.WriteLine( "An exception occurred during the processing of an application. Sad faec" );
        }

        public override void CompleteResponse()
        {
            Console.WriteLine( "Dumb-face asked for a fav.ico" );
            Response
                .Submit( HttpStatus.NoContent );
        }
    }

    public class HelloWorld : Application
    {
        public override bool HandleRequestSegment(ArraySegment<byte> data,Action continuation)
        {
            Console.WriteLine( "Got some datums for you!" );
            return base.HandleRequestSegment(data,continuation);
        }

        public override void OnError( Exception exception )
        {
            Console.WriteLine( "An exception occurred during the processing of an application. Sad faec" );
        }

        public override void CompleteResponse()
        {
            Response
                .AppendToBody( "Hellizzle Wizzizzorld! It's da HTTPizzleDizzle Fo' Rizzle, Fo' Shizzle!\r\n" )
                .Submit( HttpStatus.Ok );
        }
    }
}
