using System;
using System.Globalization;
using UnityEngine;

namespace PurrNet.Packing
{
    [Serializable]
    public struct CompressedFloat : IEquatable<CompressedFloat>
    {
        public const float PRECISION = 0.001f;

        public int rounded { get; }

        public float value => rounded * PRECISION;

        public CompressedFloat(float value)
        {
            this.rounded = Mathf.RoundToInt(value / PRECISION);
        }

        public CompressedFloat(int value)
        {
            this.rounded = value;
        }

        [Obsolete("This method is not needed anymore")]
        public CompressedFloat Round()
        {
            return this;
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator CompressedFloat(float value) => new CompressedFloat(value);
        public static implicit operator float(CompressedFloat angle) => angle.value;

        public static implicit operator CompressedFloat(int value) => new CompressedFloat(value);

        public static implicit operator CompressedFloat(PackedInt value) => new CompressedFloat(value.value * PRECISION);
        public static implicit operator PackedInt(CompressedFloat angle) => new PackedInt(Mathf.RoundToInt(angle.value / PRECISION));

        public PackedInt ToPackedInt()
        {
            return Mathf.RoundToInt(value / PRECISION);
        }

        public bool Equals(CompressedFloat other)
        {
            return rounded == other.rounded;
        }

        public override bool Equals(object obj)
        {
            return obj is CompressedFloat other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
