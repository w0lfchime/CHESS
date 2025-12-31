using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace PurrNet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true), Preserve]
    public class RegisterNetworkTypeAttribute : PreserveAttribute
    {
        public RegisterNetworkTypeAttribute([UsedImplicitly] Type type)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum), Preserve]
    public class DontPackAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field), Preserve]
    public class DontDeltaCompressAttribute : Attribute { }
}
