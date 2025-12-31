using PurrNet.Packing;

namespace PurrNet.Profiler.Deltas
{
    public abstract class DeltaProfiler
    {
        public const int MAX_ITERATIONS = 25;

        public abstract void EvaluateValue(int index, BitPacker packer, EvaluationMode mode);

        public abstract int GetPackedSize(int a, int b, EvaluationMode mode);

        public abstract string ToString(int a, EvaluationMode mode);

        public abstract double EvaluateHeight(int a, EvaluationMode mode);

        public int GetSize(int index, EvaluationMode mode)
        {
            using var packer = BitPackerPool.Get();
            EvaluateValue(index, packer, mode);
            return packer.positionInBits;
        }
    }

    public abstract class DeltaProfiler<T> : DeltaProfiler
    {
        public override int GetPackedSize(int a, int b, EvaluationMode mode)
        {
            using var delta = BitPackerPool.Get();

            var aValue = Evaluate(a, mode);
            var bValue = Evaluate(b, mode);

            DeltaPacker<T>.Write(delta, aValue, bValue);

            return delta.positionInBits;
        }

        public T Evaluate(int a, EvaluationMode mode)
        {
            using var aPacker = BitPackerPool.Get();
            EvaluateValue(a, aPacker, mode);

            aPacker.ResetPositionAndMode(true);

            T aValue = default;
            Packer<T>.Read(aPacker, ref aValue);

            return aValue;
        }

        public override string ToString(int a, EvaluationMode mode)
        {
            return Evaluate(a, mode).ToString();
        }
    }
}
