using System;
using hotstack.Owin.Impl;

namespace hotstack
{
    public interface IBuildResponse
        : IDisposable
    {
        void Submit( string status );
        void Submit( HttpStatus status );
    }
}