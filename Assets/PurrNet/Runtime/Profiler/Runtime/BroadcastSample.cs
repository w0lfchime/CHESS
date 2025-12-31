using System;
using PurrNet.Packing;

namespace PurrNet.Profiler
{
    public readonly struct BroadcastSample : IDisposable, IEquatable<BroadcastSample>
    {
        public readonly Type type;
        public readonly BitPacker data;

        public BroadcastSample(Type type, ArraySegment<byte> data)
        {
            this.type = type;
            this.data = BitPackerPool.Get();
            this.data.WriteBytes(data);
        }

        public void Dispose()
        {
            data?.Dispose();
        }

        public bool Equals(BroadcastSample other)
        {
            return type == other.type && Equals(data, other.data);
        }

        public override bool Equals(object obj)
        {
            return obj is BroadcastSample other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, data);
        }
    }
}
