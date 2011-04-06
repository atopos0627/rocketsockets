using System;
using System.Collections.Generic;

namespace hotstack.Owin
{
    public delegate void OwinApplication(
        IDictionary<string, object> request,
        OwinResponse response,
        Action<Exception> exception);
}