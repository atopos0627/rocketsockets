using System;
using hotstack.Owin.Http;

namespace hotstack.Owin
{
    public interface IBuildResponse
        : IDisposable
    {
        void Submit( string status );
        void Submit( HttpStatus status );
    }
}