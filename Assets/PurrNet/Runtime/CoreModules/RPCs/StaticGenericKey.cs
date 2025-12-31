using System;

namespace PurrNet.Modules
{
    internal readonly struct StaticGenericKey : IEquatable<StaticGenericKey>
    {
        readonly IntPtr _type;
        readonly string _methodName;
        readonly int _typesHash;

        public StaticGenericKey(IntPtr type, string methodName, Type[] types)
        {
            _type = type;
            _methodName = methodName;

            _typesHash = 0;

            for (int i = 0; i < types.Length; i++)
                _typesHash ^= types[i].GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_type, _methodName, _typesHash);
        }

        public bool Equals(StaticGenericKey other)
        {
            return _type.Equals(other._type) && _methodName == other._methodName && _typesHash == other._typesHash;
        }

        public override bool Equals(object obj)
        {
            return obj is StaticGenericKey other && Equals(other);
        }
    }
}