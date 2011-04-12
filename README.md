# About

The rocketsockets project is an attempt to create a simple, yet scalable socket server based on event loops. It was created specifically to power OWIN hosts with abstracted, sockety goodness so that as projects like Joyent's liboio mature, they can be used to improve the performance of rocketsockets without having to change how OWIN hosts talk to the network layer.

# So Simple

Start the server by calling the Start method on the ISocketServer interface which takes two callbacks:

-  OnConnectionReceived - Func<ISocketHandle, Action> where ISocketHandle provides access to the socket and Action is a callback in your code that will be called in the event the socket is closed.

-  OnBytesReceived - Action<string, ArraySegement<byte>> where string is the Id of the client socket that the bytes came from and the array segment contains bytes read.

These two callbacks are just about all your application needs. Let's look at the ISocketHandle interface. It has 1 property and 3 methods:

-  Id - get the string identity of the socket (right now, this is a Guid)
-  Close - closes the socket
-  Read - enqueues an asynchronous read on an eventloop for the socket (the OnBytesReceived gets invoked once the read completes)
-  Write - enqeues an asynchronous write on an eventloop for the socket with callbacks for write completion and exceptions

# Works with Symbiote

Already using other Symbiote libraries? Just include the using statement and extend the configuration block:

	using rocketsockets;
	using Symbiote.Daemon;
	using Symbiote.Core;
	
	namespace example
	{
		public class Program
		{
			static void Main(string[] args)
			{
				Assimilate
					.Initialize()
					.Daemon( x => x.Args( args ).Name( "example" )
					.RocketSockets( x => x.UseDefaultEndpoint() )
					.RunDaemon();
			}		
		}
	}
	
To start the rocketsocket server, take a dependency on ISocketServer and control it via the .Start() and .Stop() methods. The following example builds from the previous code block where it's assumed you're using Symbiote.Daemon. The following code block shows how you'd wire rocketsockets into an IDaemon so that it would be automatically started when the service runs. The following service will simply write all events to the console.

	public class ExampleService : IDaemon
	{
		public ISocketServer SocketServer { get; set; }
		
		public void BytesReceived( string id, ArraySegment<byte> bytes )
		{
			Console.WriteLine( "{0} bytes received from socket {1}.", bytes.Count, id );
		}
		
		public void SocketClosed( string id )
		{
			Console.WriteLine( "Socket {0} closed." );
		}
		
		public Action SocketConnected( ISocketHandle socket )
		{
			Console.WriteLine( "Socket {0} connected.", socket.Id );
			return () => SocketClosed( socket.Id );
		}
			
		public void Start()
		{
			SocketServer.Start();
		}
		
		public void Stop()
		{
			SocketServer.Stop();
		}
		
		public ExampleService( ISocketServer socketServer )
		{
			SocketServer = socketServer;
		}
	}
	
# Roadmap

-  Introduce load-balanced event-loops.
-  Finish a native sockets implementation.
-  Add liboio support.
-  Add more blinky lights and science sounds!

# Dependencies

-  Symbiote.Core
-  A Symbiote IoC adapter (Symbiote.StructureMap is available now)

# Want to Contribute?

Would love to see improvements to this, primarily around performance. PLEASE FORK IT! Also, feel free to let me know if you have comments or would like help: alex AT sharplearningcurve DOT com. You can also follow me on Twitter if you like rambling geeks: @A_Robson.