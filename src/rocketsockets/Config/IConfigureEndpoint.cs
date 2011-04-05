namespace rocketsockets
{
    public interface IConfigureEndpoint
    {
        IConfigureEndpoint BindTo( params string[] endpoints );
        IConfigureEndpoint Port( int port );
        IConfigureEndpoint SecureSockets();
    }
}