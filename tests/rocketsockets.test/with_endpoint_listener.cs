using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Concurrency;

namespace rocketsockets.test
{
    public class with_endpoint_configurator
    {
        protected static IEndpointConfiguration testConfiguration;
        protected static IConfigureEndpoint configurator;

        private Establish context = () => { 
            configurator = new EndpointConfigurator( "temp" );
            configurator.BindTo("127.0.0.1").BindToAll().Port( 10981 ).SecureSockets();
            testConfiguration = ( configurator as EndpointConfigurator ).Configuration;
        };
    }

    public class when_using_fluent_configuration_for_endpoint
        : with_endpoint_configurator
    {
        private It should_have_correct_name = () => testConfiguration.Name.ShouldEqual( "temp" );
        private It should_have_correct_ip = () => testConfiguration.BindTo.ShouldEqual( "127.0.0.1" );
        private It should_have_correct_port = () => testConfiguration.Port.ShouldEqual( 10981 );
        private It should_bind_to_any = () => testConfiguration.AnyInterface.ShouldBeTrue();
    }

    public class with_server_configurator
    {
        protected static IConfigurator configurator;
        protected static IServerConfiguration testConfiguration;

        private Establish context = () => { 
            configurator = new ServerConfigurator();
        };
    }

    public class with_default_endpoint : with_server_configurator
    {
        private Establish context = () => { 
            configurator.UseDefaultEndpoint();
            testConfiguration = ( configurator as ServerConfigurator ).Configuration;
        };
    }

    public class when_using_default_endpoint : with_default_endpoint
    {
        private It should_have_one_endpoint = () => testConfiguration.Endpoints.Count.ShouldEqual( 1 );
        private It should_be_named_default = () => testConfiguration.Endpoints[0].Name.ShouldEqual( "default" );
        private It should_bind_to_all = () => testConfiguration.Endpoints[0].AnyInterface.ShouldBeTrue();
        private It should_use_port_8998 = () => testConfiguration.Endpoints[0].Port.ShouldEqual( 8998 );
        private It should_not_use_ssl = () => testConfiguration.Endpoints[0].SSL.ShouldBeFalse();
    }

    public class with_managed_socket_listener : with_default_endpoint
    {
        protected static IEventLoop loop;
        protected static ISocketListener listener;

        private Establish context = () => { 
            loop = new EventLoopStub();
            listener = new ManagedSocketListener( loop, testConfiguration.Endpoints[0], testConfiguration );
        };
    }

    public class EventLoopStub : IEventLoop
    {
        public void Enqueue( Action action )
        {
            action();
        }

        public void Start( int workers )
        {
            
        }

        public void Stop()
        {
            
        }
    }

    public class when_closing_listener : with_managed_socket_listener
    {
        private Because of = () => listener.Close();

        private It should_be_running = () => listener.Listening.ShouldBeFalse();
    }

    public class when_connecting_to_listener : with_managed_socket_listener
    {
        static bool connectionReceived;
        static bool wasListening;
        static bool isClosed;
        static ISocket socket;

        private Because of = () => { 
            listener.ListenTo( x => 
            {
                connectionReceived = true;
                socket = x;
                x.AddCloseCallback( 
                    () => isClosed = true
                );
            } ); 
            
            using( var client = new TcpClient() ) 
            {
                client.Connect( "localhost", 8998 );
                client.Close();
            }

            wasListening = listener.Listening;
            socket.Close();
            listener.Close();
        };

        private It should_have_listened = () => wasListening.ShouldBeTrue();
        private It should_have_marked_connection_as_received = () => connectionReceived.ShouldBeTrue();
        private It should_be_closed = () => isClosed.ShouldBeTrue();
        private It should_have_stopped = () => listener.Listening.ShouldBeFalse();
    }

    public class SocketStub : ISocket
    {
        public Action OnClose { get; set; }
        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public Action<Exception> OnException { get; set; }
        public Action OnWriteComplete { get; set; }
        public Action<Exception> OnWriteException { get; set; }

        public void Dispose()
        {
            OnClose();
            OnClose = null;
            OnBytes = null;
            OnException = null;
            OnWriteComplete = null;
            OnWriteException = null;
        }

        public string Id { get; set; }

        public void AddCloseCallback( Action onClose )
        {
            OnClose = onClose;
        }

        public void Close()
        {
            Dispose();
        }

        public void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException )
        {
            OnBytes = onBytes;
            OnException = onException;
        }

        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            OnWriteComplete = onComplete;
            OnWriteException = onException;
        }
    }

    public class with_socket_handle
    {
        protected static ISocketHandle handle;
        protected static SocketStub socket;
        protected static IEventLoop loop;
        protected static OnBytesReceived onBytesReceived;
        protected static bool bytesRead;
        protected static bool underlyingSocketClosed;

        private Establish context = () => { 
            loop = new EventLoopStub();
            socket = new SocketStub() { Id  = "stub" };
            socket.AddCloseCallback( () => underlyingSocketClosed = true );
            onBytesReceived = ( id, bytes ) => bytesRead = true;
            handle = new SocketHandle( socket, loop, loop, loop, onBytesReceived );
        };
    }

    public class when_reading_from_socket_handle : with_socket_handle
    {
        private Because of = () => { 
            handle.Read();
            socket.OnBytes( new ArraySegment<byte>( new byte[] { }));
        };

        private It should_have_read_bytes = () => bytesRead.ShouldBeTrue();
    }

    public class when_reading_causes_exception : with_socket_handle
    {
        private Because of = () => { 
            handle.Read();
            socket.OnException( new Exception("this is a test"));
        };

        private It should_close_underlying_socket = () => underlyingSocketClosed.ShouldBeTrue();
    }

    public class when_write_succeeds : with_socket_handle
    {
        protected static bool writeCompleted;

        private Because of = () => { 
            handle.Write( new ArraySegment<byte>(), () => writeCompleted = true, x => { } );
            socket.OnWriteComplete();
        };

        private It should_show_write_complete = () => writeCompleted.ShouldBeTrue();
    }

    public class when_write_fails : with_socket_handle
    {
        protected static bool writeCompleted;
        protected static bool exceptionOccurred;

        private Because of = () => { 
            handle.Write( new ArraySegment<byte>(), () => writeCompleted = true, x => exceptionOccurred = true );
            socket.OnWriteException( new Exception("test") );
        };

        private It should_show_write_complete = () => exceptionOccurred.ShouldBeTrue();
        private It should_not_call_write_complete = () => writeCompleted.ShouldBeFalse();
    }
}
