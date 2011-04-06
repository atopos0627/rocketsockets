using System;
using hotstack.Owin;

namespace hotstack
{
    public interface IApplication
        : IOwinObserver
    {
        Action Cancel { get; set; }
        bool RequestCompleted { get; set; }
        IRequest Request { get; set; }
        IBuildResponse Response { get; set; }
        void Process( IRequest request, IBuildResponse response, Action<Exception> onException );
    }
}