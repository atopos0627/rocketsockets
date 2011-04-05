using System;
using System.Collections.Concurrent;

namespace rocketsockets
{
    public class SocketNode :
        LoopNode<SocketNode>,
        ISocket
    {
        public bool Available { get; set; }
        public bool PendingRead { get; protected set; }
        public bool PendingWrite { get; protected set; }		
        public ConcurrentQueue<Tuple<Action<ArraySegment<byte>>, Action<Exception>>> ReadQueue { get; set; }
        public ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>> WriteQueue { get; set; }
        public ISocket Connection { get; set; }
        public object OperationLock { get; set; }
		
        public void Close() 
        {
            Remove();
            Connection.Close();
        }

        public void Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException )
        {
            ReadQueue.Enqueue( Tuple.Create( onBytes, onException ) );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            WriteQueue.Enqueue( Tuple.Create( segment, onComplete, onException ) );
        }
		
        public bool ExecuteNextRead()
        {
            var readExecuted = false;
            lock( OperationLock )
            {
                if( Available && ReadQueue.Count > 0 )
                {
                    Available = false;
                    Tuple<Action<ArraySegment<byte>>, Action<Exception>> read = null;
                    if( ReadQueue.TryDequeue( out read ) )
                    {
                        Connection.Read( 
                            x => {
                                     Available = true;
                                     read.Item1( x );
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
				
        public SocketNode( ISocket socket )
        {
            Connection = socket;
        }
    }
}