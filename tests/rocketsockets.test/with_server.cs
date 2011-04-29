using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using rocketsockets.Impl;
using rocketsockets.test.Stubs;

namespace rocketsockets.test
{
    public class with_server : with_default_server_setup
    {
        protected static ISocketServer server;
        protected static SchedulerStub scheduler;
        protected static ListenerStub listener;
        protected static SocketStub socket;
        protected static ListenerStubFactory listenerFactory;


        private Establish context = () => { 

            listener = new ListenerStub();
            listenerFactory = new ListenerStubFactory( listener );
            scheduler = new SchedulerStub();
            socket = new SocketStub();

            server = new SocketServer( testConfiguration, listenerFactory, scheduler );
        };
    }

    public class when_starting_server : with_server
    {
        private Because of = () => server.Start( h => () => { }, (id, b) => { } );
            
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
    }

    public class when_stopping_server : with_server
    {
        private Because of = () =>
        { 
            server.Start( h => () => { }, ( id, b ) => { } ); 
            server.Stop();
        };
        
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
        private It should_have_stopped_listener = () => listener.Closed.ShouldBeTrue();
        private It should_have_stopped_scheduler = () => scheduler.Stopped.ShouldBeTrue();
    }

    public class when_testing_server_interactions : with_server
    {
        public static bool receivedHandle;
        public static bool receivedBytes;
        public static bool closeHandleCalled;

        private Because of = () =>
        { 
            server.Start( 
                h =>
                {
                    receivedHandle = true;
                    h.Read();
                    return () => { closeHandleCalled = true; };
                }, 
                ( id, b ) => 
                { 
                    receivedBytes = true; 
                } );

            listener.OnSocket( socket );
            socket.OnBytes( new ArraySegment<byte>() );

            server.Stop();
        };
        
        private It should_have_started_listener = () => listener.Listening.ShouldBeTrue();
        private It should_have_started_scheduler = () => scheduler.Started.ShouldBeTrue();
        private It should_have_stopped_listener = () => listener.Closed.ShouldBeTrue();
        private It should_have_stopped_scheduler = () => scheduler.Stopped.ShouldBeTrue();
        private It should_have_received_a_handle = () => receivedHandle.ShouldBeTrue();
        private It should_have_received_byte = () => receivedBytes.ShouldBeTrue();
    }
}
