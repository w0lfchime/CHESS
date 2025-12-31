using System;
using System.Collections.Generic;

namespace PurrNet.Pooling
{
    public class ListPool<T> : GenericPool<List<T>>
    {
#if UNITY_EDITOR
        [ThreadStatic]
#endif
        private static ListPool<T> _instance;

        static ListPool() => _instance = new ListPool<T>();

        static List<T> Factory() => new List<T>();

        static void Reset(List<T> list) => list.Clear();

        public ListPool() : base(Factory, Reset)
        {
        }

        public static int GetCount() => _instance.count;

        public static List<T> Instantiate()
        {
#if UNITY_EDITOR
            _instance ??= new ListPool<T>();
#endif
            var allocated =  _instance.Allocate();
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.Track(allocated);
#endif
            return allocated;
        }

        public static void Destroy(List<T> list)
        {
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.UnTrack(list);
#endif
            _instance.Delete(list);
        }
    }
}
