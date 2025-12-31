using System.Collections.Generic;

namespace PurrNet.Pooling
{
    public class StackPool<T> : GenericPool<Stack<T>>
    {
        private static readonly StackPool<T> _instance;

        static StackPool() => _instance = new StackPool<T>();

        static Stack<T> Factory() => new();

        static void Reset(Stack<T> list) => list.Clear();

        public StackPool() : base(Factory, Reset)
        {
        }

        public static int GetCount() => _instance.count;

        public static Stack<T> Instantiate() => _instance.Allocate();

        public static void Destroy(Stack<T> list) => _instance.Delete(list);
    }
}