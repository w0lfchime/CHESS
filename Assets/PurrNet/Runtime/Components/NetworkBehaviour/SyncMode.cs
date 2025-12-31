using JetBrains.Annotations;

namespace PurrNet
{
    public enum SyncMode : byte
    {
        No,
        World,
        [UsedImplicitly] Local
    }
}