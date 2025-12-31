using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace PurrNet
{
    public enum GuardFailureAction
    {
        ReturnDefault,
        ThrowException,
        LogWarning,
        LogError,
        Ignore,
    }

    public enum GuardFailureActionOverride
    {
        Settings,
        ReturnDefault,
        ThrowException,
        LogWarning,
        LogError,
        Ignore,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class GuardFailureActionAttribute : PreserveAttribute
    {
        [UsedImplicitly]
        public GuardFailureActionAttribute(GuardFailureActionOverride guardFailureAction = GuardFailureActionOverride.Settings) { }
    }
}