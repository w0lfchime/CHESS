using System;
using PurrNet.Logging;
using PurrNet.Packing;
using PurrNet.Transports;
using PurrNet.Utils;

namespace PurrNet.Modules
{
    public readonly struct DeltaValue : IDisposable, IEquatable<DeltaValue>
    {
        public readonly BitPacker container;

        public readonly ByteData payload;

        public readonly uint type;

        private DeltaValue(BitPacker container, uint type)
        {
            this.container = container;
            this.payload = container.ToByteData();
            this.type = type;
        }

        public static DeltaValue FromValue<T>(T value) where T : struct
        {
            var container = BitPackerPool.Get();
            Packer<T>.Write(container, value);
            return new DeltaValue(container, Hasher.GetStableHashU32<T>());
        }

        public static DeltaValue FromValue(Type type, object value)
        {
            var container = BitPackerPool.Get();
            Packer.Write(container, type, value);
            return new DeltaValue(container, Hasher.GetStableHashU32(type));
        }

        public object Deserialize(Type type)
        {
            var pos = container.positionInBits;
            container.ResetPositionAndMode(true);
            object value = null;
            Packer.Read(container, type, ref value);
            container.SetBitPosition(pos);
            return value;
        }

        public void Deserialize<T>(ref T value)
        {
            var t = Hasher.GetStableHashU32<T>();

            if (t != type)
            {
                throw new InvalidOperationException(
                    PurrLogger.FormatMessage($"Type mismatch: expected {type}, got {t}"));
            }

            var pos = container.positionInBits;
            container.ResetPositionAndMode(true);
            Packer<T>.Read(container, ref value);
            container.SetBitPosition(pos);
        }

        public T Deserialize<T>()
        {
            var t = Hasher.GetStableHashU32<T>();

            if (t != type)
            {
                throw new InvalidOperationException(
                    PurrLogger.FormatMessage($"Type mismatch: expected {type}, got {t}"));
            }

            var pos = container.positionInBits;
            container.ResetPositionAndMode(true);
            var value = default(T);
            Packer<T>.Read(container, ref value);
            container.SetBitPosition(pos);
            return value;
        }

        public void Dispose()
        {
            container?.Dispose();
        }

        public bool Equals(DeltaValue other)
        {
            return payload.Equals(other.payload) && type == other.type;
        }

        public override bool Equals(object obj)
        {
            return obj is DeltaValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return payload.GetHashCode();
        }
    }
}
