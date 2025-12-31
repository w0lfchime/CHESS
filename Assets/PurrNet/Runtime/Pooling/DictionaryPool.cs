using System;
using System.Collections.Generic;

namespace PurrNet.Pooling
{
    public class DictionaryPool<TKey, TValue> : GenericPool<Dictionary<TKey, TValue>>
    {
#if UNITY_EDITOR
        [ThreadStatic]
#endif
        private static DictionaryPool<TKey, TValue> _instance;

        static DictionaryPool() => _instance = new DictionaryPool<TKey, TValue>();

        static Dictionary<TKey, TValue> Factory() => new ();

        static void Reset(Dictionary<TKey, TValue> dic) => dic.Clear();

        private DictionaryPool() : base(Factory, Reset) { }

        public static Dictionary<TKey, TValue> Instantiate()
        {
#if UNITY_EDITOR
            _instance ??= new DictionaryPool<TKey, TValue>();
#endif
            var allocated = _instance.Allocate();
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.Track(allocated);
#endif
            return allocated;
        }

        public static void Destroy(Dictionary<TKey, TValue> list)
        {
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.UnTrack(list);
#endif
            _instance ??= new DictionaryPool<TKey, TValue>();
            _instance.Delete(list);
        }
    }
}
