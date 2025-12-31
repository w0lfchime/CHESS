using System;
using JetBrains.Annotations;

namespace PurrNet
{
    [Flags]
    [Serializable]
    public enum TransformSyncMode : byte
    {
        [UsedImplicitly] None,
        Position = 1,
        Rotation = 2,
        Scale = 4
    }
}