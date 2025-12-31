using JetBrains.Annotations;
using PurrNet.Packing;
using UnityEngine;

namespace PurrNet.Profiler.Deltas
{
    [UsedImplicitly]
    public class NetworkIDProfile : DeltaProfiler<NetworkID>
    {
        public override void EvaluateValue(int index, BitPacker packer, EvaluationMode mode)
        {
            using (new ScopedRandom(index * 1000))
            {
                var value = mode switch
                {
                    EvaluationMode.PerlinNoise => new NetworkID((ulong) Mathf.PerlinNoise(index * 0.01f, 0) * 100),
                    EvaluationMode.Linear => new NetworkID((ulong)(index * 2)),
                    EvaluationMode.Quadratic => new NetworkID((ulong)( index * index)),
                    EvaluationMode.Cubic => new NetworkID((ulong)(index * index * index)),
                    EvaluationMode.Exponential => new NetworkID((ulong)(Mathf.Pow(2, index) * 0.01f)),
                    EvaluationMode.Random => new NetworkID((ulong)Random.Range(0, 10000)),
                    _ => default
                };

                Packer<NetworkID>.Write(packer, value);
            }
        }

        public override double EvaluateHeight(int a, EvaluationMode mode)
        {
            return Evaluate(a, mode).id;
        }
    }
}
