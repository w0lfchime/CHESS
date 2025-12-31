namespace PurrNet.Packing
{
    public struct DeltaWritingScope
    {
        private readonly BitPacker packer;
        private readonly int hasChangedFlag;
        private bool hasChanged;

        public DeltaWritingScope(BitPacker packer)
        {
            hasChangedFlag = packer.AdvanceBits(1);
            this.packer = packer;
            hasChanged = false;
        }

        public void Write<T>(T value)
        {
            Packer<T>.Write(packer, value);
            hasChanged = true;
        }

        public void Write<T>(T oldvalue, T newvalue)
        {
            hasChanged |= DeltaPacker<T>.Write(packer, oldvalue, newvalue);
        }

        public bool CompleteWithoutChanges()
        {
            packer.WriteAt(hasChangedFlag, false);
            packer.SetBitPosition(hasChangedFlag + 1);
            return false;
        }

        public bool CompleteWithChanges()
        {
            packer.WriteAt(hasChangedFlag, true);
            return true;
        }

        public bool Complete()
        {
            packer.WriteAt(hasChangedFlag, hasChanged);
            if (!hasChanged)
                packer.SetBitPosition(hasChangedFlag + 1);
            return hasChanged;
        }

        public void MarkDirty()
        {
            hasChanged = true;
        }
    }
}
