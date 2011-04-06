using System;
using System.Collections.Concurrent;

namespace rocketsockets
{
    public class SocketNode :
        LoopNode<SocketNode>,
        ISocketHandle
    {
        public string Id { get; set; }
        public bool Available { get; set; }
        public ISocket Connection { get; set; }
        public bool PendingRead { get; set; }
        public bool PendingWrite { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public object OperationLock { get; set; }
        public ConcurrentQueue<Tuple<OnBytesReceived, Action<Exception>>> ReadQueue { get; set; }
        public ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>> WriteQueue { get; set; }

        public void Close() 
        {
            Remove();
            Connection.Close();
        }

        public bool ExecuteNextRead()
        {
            var readExecuted = false;
            lock( OperationLock )
            {
                if( Available && ReadQueue.Count > 0 )
                {
                    Available = false;
                    Tuple<OnBytesReceived, Action<Exception>> read = null;
                    if( ReadQueue.TryDequeue( out read ) )
                    {
                        Connection.Read( 
                            x => {
                                     Available = true;
                                     read.Item1( Id, x );
                            },
                            x => {
                                     Available = true;
                                     read.Item2( x );
                            });
                        readExecuted = true;
                    }
                }
            }
            return readExecuted;
        }
		
        public bool ExecuteNextWrite()
        {
            var writeExecuted = false;
            lock( OperationLock )
            {
                if( Available && WriteQueue.Count > 0 )
                {
                    Available = false;
                    Tuple<ArraySegment<byte>, Action, Action<Exception>> write = null;
                    if( WriteQueue.TryDequeue( out write ) )
                    {
                        Connection.Write(
                            write.Item1,
                            () => {
                                      Available = true;
                                      write.Item2();
                            },
                            x => {
                                     Available = true;
                                     write.Item3( x );
                            });
                        writeExecuted = true;
                    }
                }
            }
            return writeExecuted;
        }

        public void HandleReadException( Exception exception ) 
        {

        }

        public void Read()
        {
            ReadQueue.Enqueue( Tuple.Create( OnBytes, (Action<Exception>) HandleReadException ) );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            WriteQueue.Enqueue( Tuple.Create( segment, onComplete, onException ) );
        }
		
        public SocketNode( string id, ISocket socket, OnBytesReceived onBytes )
        {
            Id = id;
            OnBytes = onBytes;
            Connection = socket;
            ReadQueue = new ConcurrentQueue<Tuple<OnBytesReceived, Action<Exception>>>();
            WriteQueue = new ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>>();
        }
    }
}