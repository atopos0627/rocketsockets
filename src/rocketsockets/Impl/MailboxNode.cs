using System;
using System.Collections.Concurrent;
using System.Threading;

namespace rocketsockets
{
    public class MailboxNode :
        LoopNode<MailboxNode>
    {
        public string Id { get; set; }
        public bool Processing { get; set; }
        public object ProcessLock { get; set; }
        public ConcurrentQueue<ArraySegment<byte>> Messages { get; set; }

        public void Write( ArraySegment<byte> message )
        {
            Messages.Enqueue( message );
        }

        public void Process( Action<ArraySegment<byte>> action )
        {
            if( !Processing && Messages.Count > 0 )
            {
                try
                {
                    Processing = true;
                    lock( ProcessLock )
                    {
                        ArraySegment<byte> message;
                        do
                        {
                            if ( Messages.TryDequeue( out message ) )
                                try
                                {
                                    action( message );
                                }
                                catch( Exception e )
                                {
                                    var x = e;
                                }
                                finally
                                {
                                    
                                }
                        } while ( Messages.Count > 0 );
                    }
                }
                finally
                {
                    Processing = false;
                }
            }
            else
            {
                Thread.Sleep( 0 );
            }
        }

        public MailboxNode( string id )
        {
            Id = id;
            ProcessLock = new object();
            Messages = new ConcurrentQueue<ArraySegment<byte>>();
        }
    }
}