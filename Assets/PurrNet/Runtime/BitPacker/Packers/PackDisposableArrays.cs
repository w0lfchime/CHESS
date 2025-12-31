using PurrNet.Pooling;

namespace PurrNet.Packing
{
    public static class PackDisposableArrays
    {
        public static bool WriteDisposableArrayDelta<T>(BitPacker packer, DisposableArray<T> oldValue, DisposableArray<T> newValue)
        {
            using var oldDisposableList = oldValue.isDisposed ? default : DisposableList<T>.Create(oldValue);
            using var newDisposableList = newValue.isDisposed ? default : DisposableList<T>.Create(newValue);
            return DeltaPacker<DisposableList<T>>.Write(packer, oldDisposableList, newDisposableList);
        }

        public static void ReadDisposablArrayDelta<T>(BitPacker packer, DisposableArray<T> oldValue, ref DisposableArray<T> value)
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
                value = DisposableArray<T>.Create(newDisposableList);
                newDisposableList.Dispose();
                return;
            }

            if (value.Count != newDisposableList.Count)
                value.Resize(newDisposableList.Count);

            newDisposableList.CopyTo(value.array, 0);
            newDisposableList.Dispose();
        }
    }
}
