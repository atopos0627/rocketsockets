using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hotstack 
{
    public class MissingApplicationException : Exception
    {
        public MissingApplicationException( string message ) : base( message ) {}
        public MissingApplicationException( string message, Exception innerException ) : base( message, innerException ) {}
    }
}
