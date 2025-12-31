using System;
using System.Reflection;
using PurrNet.Packing;

namespace PurrNet.Modules
{
    internal readonly struct RPCKey : IEquatable<RPCKey>
    {
        private readonly IReflect type;
        private readonly Size rpcId;

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ rpcId.GetHashCode();
        }

        public RPCKey(IReflect type, Size rpcId)
        {
            this.type = type;
            this.rpcId = rpcId;
        }

        public bool Equals(RPCKey other)
        {
            return Equals(type, other.type) && rpcId.value == other.rpcId.value;
        }

        public override bool Equals(object obj)
        {
            return obj is RPCKey other && Equals(other);
        }
    }
}
