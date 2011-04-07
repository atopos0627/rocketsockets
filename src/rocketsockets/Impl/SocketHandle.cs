using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;

namespace rocketsockets
{
    public class SocketHandle :
        ISocketHandle
    {
        public string Id { get; set; }
        public bool Removed { get; set; }
        public bool Available { get { return !Removed && ( !PendingRead || !PendingWrite ) && ( ReadCount > 0 || WriteCount > 0 ); } }
        public ISocket Connection { get; set; }
        public ISocketLoop Loop { get; set; }
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
            Removed = true;
            Loop = null;
            PendingRead = false;
            PendingWrite = false;
            OnBytes = null;
            ReadCount = 0;
            WriteCount = 0;
            ReadQueue = null;
            WriteQueue = null;
            Connection.Close();
            Connection = null;
        }

        public void ExecuteNextRead()
        {
            var readExecuted = false;
            if( !PendingRead && ReadCount > 0 )
            {
                lock( ReadLock )
                {
                    Tuple<OnBytesReceived, Action<Exception>> read = null;

                    try
                    {
                        if( ReadQueue.TryDequeue( out read ) )
                        {
                            ReadCount--;
                            PendingRead = true;
                            "Begging read on socket {0}"
                                .ToDebug<ISocketHandle>(Id);
                            Connection.Read( 
                                x => {
                                         read.Item1( Id, x );
                                         PendingRead = false;
                                },
                                x => {
                                         PendingRead = false;
                                         read.Item2( x );
                                });
                            readExecuted = true;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }
		
        public void ExecuteNextWrite()
        {
            var writeExecuted = false;
            if( !PendingWrite && WriteCount > 0 )
            {
                lock( WriteLock )
                {
                    Tuple<ArraySegment<byte>, Action, Action<Exception>> write = null;
                    if( WriteQueue.TryDequeue( out write ) )
                    {
                        WriteCount--;
                        PendingWrite = true;
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
        }

        public void HandleReadException( Exception exception ) 
        {

        }

        public void Read()
        {
            ReadCount++;
            ReadQueue.Enqueue( Tuple.Create( OnBytes, (Action<Exception>) HandleReadException ) );
            Loop.Enqueue( ExecuteNextRead );
        }
		
        public void Write( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException )
        {
            //Connection.Write( segment, onComplete, onException );
            WriteCount++;
            WriteQueue.Enqueue( Tuple.Create( segment, onComplete, onException ) );
            Loop.Enqueue( ExecuteNextWrite );
        }
		
        public SocketHandle( string id, ISocket socket, ISocketLoop loop, OnBytesReceived onBytes )
        {
            Id = id;
            OnBytes = onBytes;
            Connection = socket;
            ReadLock = new object();
            WriteLock = new object();
            Loop = loop;
            ReadQueue = new ConcurrentQueue<Tuple<OnBytesReceived, Action<Exception>>>();
            WriteQueue = new ConcurrentQueue<Tuple<ArraySegment<byte>, Action, Action<Exception>>>();
        }
    }
}