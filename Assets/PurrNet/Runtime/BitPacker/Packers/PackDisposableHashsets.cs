using PurrNet.Pooling;

namespace PurrNet.Packing
{
    public static class PackDisposableHashsets
    {
        public static bool WriteDisposableHashSetDelta<T>(BitPacker packer, DisposableHashSet<T> oldValue, DisposableHashSet<T> newValue)
        {
            using var oldDisposableList = oldValue.isDisposed ? default : DisposableList<T>.Create(oldValue);
            using var newDisposableList = newValue.isDisposed ? default : DisposableList<T>.Create(newValue);
            return DeltaPacker<DisposableList<T>>.Write(packer, oldDisposableList, newDisposableList);
        }

        public static void ReadDisposableHashSetDelta<T>(BitPacker packer, DisposableHashSet<T> oldValue, ref DisposableHashSet<T> value)
        {
            using var oldDisposableList = oldValue.isDisposed ? default : DisposableList<T>.Create(oldValue);
            DisposableList<T> newDisposableList = default;

            DeltaPacker<DisposableList<T>>.Read(packer, oldDisposableList, ref newDisposableList);

            if (newDisposableList.isDisposed)
            {
                value.Dispose();
                return;
            }

            if (value.isDisposed)
            {
                value = DisposableHashSet<T>.Create(newDisposableList);
            }
            else
            {
                value.Clear();
                value.UnionWith(newDisposableList);
            }

            newDisposableList.Dispose();
        }
    }
}
