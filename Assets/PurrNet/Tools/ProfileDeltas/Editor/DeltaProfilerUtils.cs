using System;
using System.Linq;

namespace PurrNet.Profiler.Deltas.Editor
{
    public static class DeltaProfilerUtils
    {
        public static Type[] GetAllDeltaProfilers()
        {
            return typeof(DeltaProfiler).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DeltaProfiler)) && !t.IsAbstract)
                .ToArray();
        }
    }
}
