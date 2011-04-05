namespace rocketsockets
{
    public interface IConfigureServer
    {
        void AddEndPoint( IEndpointConfiguration endpoint );
    }
}