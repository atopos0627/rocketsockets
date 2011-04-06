using System;

namespace rocketsockets
{
    public class MailboxProcessor :
        IMailboxProcessor
    {
        public MailboxNode Root { get; set; }
        public bool Running { get; set; }
        public OnBytesReceived Process { get; set; }
        public ExclusiveDictionary<string, MailboxNode> Mailboxes { get; set; }

        public void Loop()
        {
            var node = Root;
            while( Running )
            {
                if( node.Processing )
                    node.Process( Process );
                node = node.Next;
            }
        }

        public void Remove( string id ) 
        {
            var node = Mailboxes.GetOrDefault( id );
            if( node != null ) 
            {
                node.Remove();
            }
        }

        public void Start()
        {
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

        public void Write( string Id, ArraySegment<byte> bytes )
        {
            var mailboxNode = Mailboxes.ReadOrWrite( Id, () => new MailboxNode( Id ) );
            mailboxNode.Write( bytes );
            Root.AddNode( mailboxNode );
        }

        public MailboxProcessor( OnBytesReceived process ) 
        {
            Process = process;
            Root = new MailboxNode("") { Processing = true };
            Mailboxes = new ExclusiveDictionary<string, MailboxNode>();
        }
    }
}