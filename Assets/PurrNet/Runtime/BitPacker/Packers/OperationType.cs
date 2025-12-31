using PurrNet.Modules;

namespace PurrNet.Packing
{
    public enum OperationType : byte
    {
        Add,      // append only, no index
        Insert,   // insert at position
        Delete,   // range delete
        End
    }

    public static class OperationTypePacker
    {
        [UsedByIL]
        public static void WriteOperationType(this BitPacker packer, OperationType value)
        {
            byte val = (byte)value;
            packer.WriteBits(val, 2);
        }

        [UsedByIL]
        public static void ReadOperationType(this BitPacker packer, ref OperationType value)
        {
            value = (OperationType)packer.ReadBits(2);
        }

        [UsedByIL]
        public static bool DeltaWrite(this BitPacker packer, OperationType old, OperationType newVal)
        {
            var scope = new DeltaWritingScope(packer);
            if (old != newVal)
            {
                WriteOperationType(packer, newVal);
                return scope.CompleteWithChanges();
            }
            return scope.Complete();
        }

        [UsedByIL]
        public static void DeltaRead(this BitPacker packer, OperationType old, ref OperationType newVal)
        {
            if (!DeltaReadingScope.Continue(packer, old, ref newVal))
                return;

            ReadOperationType(packer, ref newVal);
        }
    }
}
