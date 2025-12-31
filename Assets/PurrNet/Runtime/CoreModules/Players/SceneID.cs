using System;
using PurrNet.Packing;

namespace PurrNet
{
    public readonly struct SceneID : IEquatable<SceneID>, IPackedAuto
    {
        private Size _id { get; }

        public Size id => _id;

        public SceneID(ushort id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id.value.ToString("000");
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public bool Equals(SceneID other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneID other && Equals(other);
        }

        public static bool operator ==(SceneID a, SceneID b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(SceneID a, SceneID b)
        {
            return a._id != b._id;
        }
    }
}
