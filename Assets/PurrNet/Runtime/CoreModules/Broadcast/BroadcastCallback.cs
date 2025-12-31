using PurrNet.Transports;

namespace PurrNet.Modules
{
    public delegate void BroadcastDelegate<in T>(Connection conn, T data, bool asServer);

    internal readonly struct BroadcastCallback<T> : IBroadcastCallback
    {
        readonly BroadcastDelegate<T> callback;

        public BroadcastCallback(BroadcastDelegate<T> callback)
        {
            this.callback = callback;
        }

        public bool IsSame(object callbackToCmp)
        {
            return callbackToCmp is BroadcastDelegate<T> action && action == callback;
        }

        public void TriggerCallback(Connection conn, object data, bool asServer)
        {
            if (data is T value)
                callback?.Invoke(conn, value, asServer);
        }

        public void Subscribe(BroadcastModule module)
        {
            module.Subscribe(callback);
        }
    }
}
