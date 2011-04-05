namespace rocketsockets
{
    public interface ISocketLoop
    {
        ISocket AddSocket( ISocket socket );
        void RemoveSocket( ISocket socket );
        void Start();
        void Stop();
    }
}