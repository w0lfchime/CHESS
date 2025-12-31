using System.Collections.Generic;

namespace PurrNet.Modules
{
    public struct DeltaMessagesList
    {
        public int nextId;
        public List<DeltaMessage> messages;
    }

    public class DeltaIDs
    {
        const int MAX_CACHE_SIZE = 8;

        readonly Dictionary<DeltaValue, List<int>> _cachedDeltas = new();

        public void Comfirm(DeltaValue key, int messageId)
        {
            if (!_cachedDeltas.TryGetValue(key, out var deltas))
            {
                deltas = new List<int>();
                _cachedDeltas[key] = deltas;
            }

            if (deltas.Count >= MAX_CACHE_SIZE)
                deltas.RemoveAt(0);

            // insert ordered
            for (var i = 0; i < deltas.Count; i++)
            {
                if (deltas[i] > messageId)
                {
                    deltas.Insert(i, messageId);
                    return;
                }
            }
        }
    }

    public class DeltaCache
    {
        const int MAX_CACHE_SIZE = 32;

        readonly Dictionary<DeltaValue, DeltaMessagesList> _cachedDeltas = new();

        public int GetNextMessageId(DeltaValue key)
        {
            if (!_cachedDeltas.TryGetValue(key, out var deltas))
            {
                deltas = new DeltaMessagesList
                {
                    messages = new List<DeltaMessage>(),
                    nextId = 0
                };

                _cachedDeltas[key] = deltas;
            }

            return deltas.nextId++;
        }

        public void Cache(DeltaMessage message)
        {
            if (!_cachedDeltas.TryGetValue(message.key, out var deltas))
            {
                deltas = new DeltaMessagesList
                {
                    messages = new List<DeltaMessage>(),
                    nextId = 0
                };

                _cachedDeltas[message.key] = deltas;
            }

            // Remove the oldest delta if the cache is full
            if (deltas.messages.Count >= MAX_CACHE_SIZE)
                deltas.messages.RemoveAt(0);

            deltas.messages.Add(message);
        }

        public bool TryGetDeltaMessage(DeltaValue key, int messageId, out DeltaMessage message)
        {
            if (_cachedDeltas.TryGetValue(key, out var deltas))
            {
                for (var i = 0; i < deltas.messages.Count; i++)
                {
                    var delta = deltas.messages[i];
                    if (delta.messageId == messageId)
                    {
                        message = delta;
                        return true;
                    }
                }
            }

            message = default;
            return false;
        }
    }
}
