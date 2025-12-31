using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace PurrNet
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ServerAttribute : PreserveAttribute
    {
        [UsedImplicitly]
        public ServerAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OwnerAttribute : PreserveAttribute
    {
        [UsedImplicitly]
        public OwnerAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ClientAttribute : PreserveAttribute
    {
        [UsedImplicitly]
        public ClientAttribute() { }
    }
}
