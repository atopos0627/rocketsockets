using System;

namespace rocketsockets
{
    public interface IEventLoop
    {
        void Enqueue( Action action );
        void Start( int workers );
        void Stop();
    }
}