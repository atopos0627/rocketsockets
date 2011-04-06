using System;
using hotstack.Owin;

namespace hotstack.Config
{
    public interface IConfigureHttp
    {
        IConfigureHttp ConfigureHost( Action<IConfigureServerSettings> configurator );
        IConfigureHttp RegisterApplications( Action<IRegisterApplication> register );
    }
}