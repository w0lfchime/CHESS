using System;

namespace PurrNet.Packing
{
    [Serializable]
    public struct PackedByte : IPackedAuto
    {
        public byte value;

        public PackedByte(byte value)
        {
            this.value = value;
        }

        public static implicit operator PackedByte(byte value) => new PackedByte(value);

        public static implicit operator byte(PackedByte value) => value.value;

        public override string ToString()
        {
            return $"{value}";
        }
    }

    [Serializable]
    public struct PackedSByte : IPackedAuto
    {
        public sbyte value;

        public PackedSByte(sbyte value)
        {
            this.value = value;
        }

        public static implicit operator PackedSByte(sbyte value) => new PackedSByte(value);

        public static implicit operator sbyte(PackedSByte value) => value.value;

        public override string ToString()
        {
            return $"{value}";
        }
    }

    [Serializable]
    public struct PackedULong : IEquatable<PackedULong>, IPackedAuto
    {
        public ulong value;

        public PackedULong(ulong value)
        {
            this.value = value;
        }

        public static implicit operator PackedULong(ulong value) => new PackedULong(value);

        public static implicit operator ulong(PackedULong value) => value.value;

        public bool Equals(PackedULong other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is PackedULong other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{value}";
        }
    }

    [Serializable]
    public struct PackedLong : IPackedAuto, IEquatable<PackedLong>
    {
        public long value;

        public PackedLong(long value)
        {
            this.value = value;
        }

        public static implicit operator PackedLong(long value) => new PackedLong(value);

        public static implicit operator long(PackedLong value) => value.value;

        public override string ToString()
        {
            return $"{value}";
        }

        public bool Equals(PackedLong other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is PackedLong other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    [Serializable]
    public struct PackedUInt : IEquatable<PackedUInt>, IPackedAuto
    {
        public uint value;

        public PackedUInt(uint value)
        {
            this.value = value;
        }

        public static implicit operator PackedUInt(uint value) => new PackedUInt(value);

        public static implicit operator uint(PackedUInt value) => value.value;

        public bool Equals(PackedUInt other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is PackedUInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)value;
        }

        public override string ToString()
        {
            return $"{value}";
        }
    }

    [Serializable]
    public struct PackedInt : IEquatable<PackedInt>, IPackedAuto
    {
        public int value;

        public PackedInt(int value)
        {
            this.value = value;
        }

        public static implicit operator PackedInt(int value) => new PackedInt(value);

        public static implicit operator int(PackedInt value) => value.value;

        public bool Equals(PackedInt other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is PackedInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public override string ToString()
        {
            return $"{value}";
        }
    }

    [Serializable]
    public struct PackedUShort : IPackedAuto, IEquatable<PackedUShort>
    {
        public ushort value;

        public PackedUShort(ushort value)
        {
            this.value = value;
        }

        public static implicit operator PackedUShort(ushort value) => new PackedUShort(value);

        public static implicit operator ushort(PackedUShort value) => value.value;

        public override string ToString()
        {
            return $"{value}";
        }

        public bool Equals(PackedUShort other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is PackedUShort other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    [Serializable]
    public struct PackedShort : IPackedAuto
    {
        public short value;

        public PackedShort(short value)
        {
            this.value = value;
        }

        public static implicit operator PackedShort(short value) => new PackedShort(value);

        public static implicit operator short(PackedShort value) => value.value;

        public override string ToString()
        {
            return $"{value}";
        }
    }
}
