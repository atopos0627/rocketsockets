using System.Collections.Generic;

namespace hotstack.Owin
{
    public delegate void OwinResponse(
        string status,
        IDictionary<string, string> headers,
        OwinBody body);
}