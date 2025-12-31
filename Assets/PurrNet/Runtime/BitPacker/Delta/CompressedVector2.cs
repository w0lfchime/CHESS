using System;
using UnityEngine;

namespace PurrNet.Packing
{
    [System.Serializable]
    public struct CompressedVector2 : IEquatable<CompressedVector2>
    {
        public CompressedFloat x;
        public CompressedFloat y;

        public CompressedVector2(CompressedFloat x, CompressedFloat y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"CompressedVector2({x}, {y})";
        }

        public static implicit operator CompressedVector2(Vector2 value) => new CompressedVector2(value.x, value.y);
        public static implicit operator Vector2(CompressedVector2 vector) => new Vector2(vector.x.value, vector.y.value);

        public bool Equals(CompressedVector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj)
        {
            return obj is CompressedVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}
