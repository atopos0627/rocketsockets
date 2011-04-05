namespace rocketsockets
{
    public interface IEndpointListener
    {
        bool Running { get; }
        void Start();
        void Stop();
    }
}