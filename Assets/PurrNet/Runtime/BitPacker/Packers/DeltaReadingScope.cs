using System;

namespace PurrNet.Packing
{
    public static class DeltaReadingScope
    {
        public static bool Continue<T>(BitPacker packer, T old, ref T newVal)
        {
            bool hasChanged = Packer<bool>.Read(packer);
            if (!hasChanged)
            {
                if (newVal is IDisposable disposable)
                    disposable.Dispose();
                newVal = Packer.Copy(old);
                return false;
            }
            return true;
        }
    }
}
