using System;
using System.Diagnostics;

namespace PurrNet.Pooling
{
    public struct AllocationDetails
    {
        public readonly StackTrace allocationTrace;
        public StackTrace lastUsageTrace;
        public readonly Type targetType;
        public readonly WeakReference<object> reference;

        public AllocationDetails(object target)
        {
            allocationTrace = new StackTrace(true);
            lastUsageTrace = allocationTrace;
            targetType = target.GetType();
            reference = new WeakReference<object>(target);
        }
    }
}
