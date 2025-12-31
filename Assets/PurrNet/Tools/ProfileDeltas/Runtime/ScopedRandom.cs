using UnityEngine;

namespace PurrNet.Profiler.Deltas
{
    public readonly struct ScopedRandom : System.IDisposable
    {
        private readonly Random.State _state;

        public ScopedRandom(int seed)
        {
            _state = Random.state;
            Random.InitState(seed);
        }

        public void Dispose()
        {
            Random.state = _state;
        }
    }
}
