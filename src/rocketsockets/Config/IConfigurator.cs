using System;

namespace rocketsockets
{
    public interface IConfigurator
    {
        IConfigurator AddEndpoint( string name, Action<IConfigureEndpoint> configurator );
        IConfigurator UseDefaultEndpoint();
    }
}