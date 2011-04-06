using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketNode :
        LoopNode<SocketNode>,
        ISocketHandle
    {
        public string Id { get; set; }
        public bool Available { get { return ( !PendingRead || !PendingWrite ) && ( ReadCount > 0 || WriteCount > 0 ); } }
        public ISocket Connection { get; set; }
        public bool PendingRead { get; set; }
        public bool PendingWrite { get; set; }
        public OnBytesReceived OnBytes { get; set; }
        public object ReadLock { get; set; }
        public object WriteLock { get; set; }
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }
        public ConcurrentQueue<Tuple<OnBytesReceived, Action<Exception>>> ReadQueue { get; set; }
        public ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>> WriteQueue { get; set; }

        public void Close() 
        {
            Connection.Close();
        }

        public bool ExecuteNextRead()
        {
            var readExecuted = false;
            if( !Available )
                return false;
            lock( ReadLock )
            {
                if( !PendingRead && ReadQueue.Count > 0 )
                {
                    PendingRead = true;
                    Tuple<OnBytesReceived, Action<Exception>> read = null;
                    if( ReadQueue.TryDequeue( out read ) )
                    {
                        ReadCount--;
                        "Begging read on socket {0}"
                            .ToDebug<ISocketHandle>(Id);
                        Connection.Read( 
                            x => {
                                     PendingRead = false;
                                     read.Item1( Id, x );
                            },
                            x => {
                                     PendingRead = false;
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
            lock( ReadLock )
            {
                if( !PendingWrite && WriteQueue.Count > 0 )
                {
                    WriteCount--;
                    PendingWrite = true;
                    Tuple<ArraySegment<byte>, Action, Action<Exception>> write = null;
                    if( WriteQueue.TryDequeue( out write ) )
                    {
                        Connection.Write(
                            write.Item1,
                            () => {
                                      PendingWrite = false;
                                      write.Item2();
                            },
                            x => {
                                     PendingWrite = false;
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

        public override void Remove()
        {
            ReadCount = 0;
            WriteCount = 0;
            base.Remove();
            Connection = null;
            OnBytes = null;
            ReadLock = null;
            WriteLock = null;
            ReadQueue = null;
            WriteQueue = null;
        }

        public void Read()
        {
            ReadCount++;
            ReadQueue.Enqueue( Tuple.Create( OnBytes, (Action<Exception>) HandleReadException ) );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            WriteCount++;
            WriteQueue.Enqueue( Tuple.Create( segment, onComplete, onException ) );
        }
		
        public SocketNode( string id, ISocket socket, OnBytesReceived onBytes )
        {
            Id = id;
            OnBytes = onBytes;
            Connection = socket;
            ReadLock = new object();
            ReadQueue = new ConcurrentQueue<Tuple<OnBytesReceived, Action<Exception>>>();
            WriteQueue = new ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>>();
        }
    }
}