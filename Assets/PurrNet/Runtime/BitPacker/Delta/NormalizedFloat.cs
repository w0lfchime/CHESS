using System;
using System.Globalization;

namespace PurrNet.Packing
{
    public struct NormalizedFloat : IEquatable<NormalizedFloat>
    {
        public const int BIT_RESOLUTION = 11;
        const double PRECISION = 1f / (1 << (BIT_RESOLUTION - 1));

        public long value;

        public override string ToString()
        {
            return GetValue().ToString(CultureInfo.InvariantCulture);
        }

        public NormalizedFloat(long value)
        {
            this.value = value;
        }

        public NormalizedFloat(float value)
        {
            const long MIN = -1 << (BIT_RESOLUTION - 1);
            const long MAX = (1 << (BIT_RESOLUTION - 1)) - 1;

            this.value = Math.Clamp((long)Math.Round(Math.Clamp(value, -1, 1) / PRECISION), MIN, MAX);
        }

        public float GetValue()
        {
            return (float)(value * PRECISION);
        }

        public static implicit operator NormalizedFloat(float value)
        {
            return new NormalizedFloat(value);
        }

        public static implicit operator float(NormalizedFloat angle)
        {
            return angle.GetValue();
        }

        public static NormalizedFloat operator -(NormalizedFloat a, NormalizedFloat b)
        {
            return new NormalizedFloat(a.value - b.value);
        }

        public static NormalizedFloat operator +(NormalizedFloat a, NormalizedFloat b)
        {
            return new NormalizedFloat(a.value + b.value);
        }

        public bool Equals(NormalizedFloat other)
        {
            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is NormalizedFloat other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
