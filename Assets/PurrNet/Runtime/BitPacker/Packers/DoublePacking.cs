using System;
using PurrNet.Modules;

namespace PurrNet.Packing
{
    public static class DoublePacking
    {
        [UsedByIL]
        public static void Write(this BitPacker packer, double data)
        {
            packer.WriteBits((ulong)BitConverter.DoubleToInt64Bits(data), 64);
        }

        [UsedByIL]
        public static void Read(this BitPacker packer, ref double data)
        {
            data = BitConverter.Int64BitsToDouble((long)packer.ReadBits(64));
        }
    }
}
