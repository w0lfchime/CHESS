using PurrNet.Transports;

namespace PurrNet.Modules
{
    public interface IBroadcastCallback
    {
        bool IsSame(object callback);

        void TriggerCallback(Connection conn, object data, bool asServer);

        void Subscribe(BroadcastModule module);
    }
}