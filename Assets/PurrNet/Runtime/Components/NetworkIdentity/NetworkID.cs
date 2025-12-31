using System;
using JetBrains.Annotations;
using PurrNet.Packing;

namespace PurrNet
{
    public struct NetworkID : IEquatable<NetworkID>
    {
        [UsedImplicitly] private PlayerID _scope;
        [UsedImplicitly] private PackedULong _id;

        public PackedULong id => _id;

        public PlayerID scope => _scope;

        public NetworkID(NetworkID baseId, ulong offset)
        {
            _id = baseId._id + offset;
            _scope = baseId._scope;
        }

        public NetworkID(ulong id, PlayerID scope = default)
        {
            _id = id;
            _scope = scope;
        }

        public override string ToString()
        {
            return $"{_scope}:{_id}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_scope, _id);
        }

        public bool Equals(NetworkID other)
        {
            return _scope == other._scope && _id == other._id;
        }

        public static bool Equals(NetworkID? a, NetworkID? b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.Value.Equals(b.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkID other && Equals(other);
        }

        public static bool operator ==(NetworkID a, NetworkID b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NetworkID a, NetworkID b)
        {
            return !a.Equals(b);
        }

        public static bool operator <(NetworkID a, NetworkID b)
        {
            if (a._scope == b._scope)
                return a._id < b._id;
            return a._scope.id < b._scope.id;
        }

        public static bool operator >(NetworkID a, NetworkID b)
        {
            if (a._scope == b._scope)
                return a._id > b._id;
            return a._scope.id > b._scope.id;
        }
    }
}
