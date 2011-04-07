using System;

namespace rocketsockets
{
    public interface ISocketLoop
    {
        void Enqueue( Action action );
        void Start();
        void Stop();
    }
}