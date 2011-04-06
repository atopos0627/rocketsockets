namespace rocketsockets
{
    public interface IConfigureEndpoint
    {
        IConfigureEndpoint BindToAll();
        IConfigureEndpoint BindTo( string endpoints );
        IConfigureEndpoint Port( int port );
        IConfigureEndpoint SecureSockets();
    }
}