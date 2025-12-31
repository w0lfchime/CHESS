using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace PurrNet
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ServerOnlyAttribute : PreserveAttribute
    {
        [UsedImplicitly]
        public ServerOnlyAttribute(StripCodeModeOverride stripCodeMode = StripCodeModeOverride.Settings) { }
    }
}
