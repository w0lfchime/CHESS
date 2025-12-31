using System;
using System.Collections.Generic;
using System.Diagnostics;
using PurrNet.Logging;

namespace PurrNet.Pooling
{
    public static class AllocationTracker
    {
        [ThreadStatic]
        static List<AllocationDetails> _allocations;

        public static void Track(object target)
        {
            _allocations ??= new List<AllocationDetails>();
            _allocations.Add(new AllocationDetails(target));
        }

        public static void UnTrack(object target)
        {
            if (_allocations == null) return;
            for (int i = 0; i < _allocations.Count; i++)
            {
                if (_allocations[i].reference.TryGetTarget(out var obj) && obj == target)
                {
                    _allocations.RemoveAt(i);
                    return;
                }
            }
            PurrLogger.LogError($"Object of type `{target.GetType()}` was not tracked but we are freeing it?");
        }

        public static void CheckForLeaks()
        {
            if (_allocations == null) return;
            for (var i = 0; i < _allocations.Count; i++)
            {
                var allocation = _allocations[i];
                if (!allocation.reference.TryGetTarget(out _))
                {
                    _allocations.RemoveAt(i--);
                    PurrLogger.LogError($"Object of type {allocation.targetType} was leaked.\n\nAllocation StackTrace: \n{allocation.allocationTrace}\n\n" +
                                        $"Last Usage StackTrace: \n{allocation.lastUsageTrace}\n\n");
                }
            }
        }

        public static void UpdateUsage(object list)
        {
            if (_allocations == null) return;
            for (var i = 0; i < _allocations.Count; i++)
            {
                var allocation = _allocations[i];
                if (allocation.reference.TryGetTarget(out var obj) && obj == list)
                {
                    var val = _allocations[i];
                    val.lastUsageTrace = new StackTrace(true);
                    _allocations[i] = val;
                }
            }
        }
    }
}
