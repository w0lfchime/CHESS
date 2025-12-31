using System;
using System.Collections.Generic;
using PurrNet.Packing;
using PurrNet.Pooling;
using PurrNet.Transports;
using UnityEngine;

namespace PurrNet.Modules
{
    public struct UnreliableAck
    {
        public Connection conn;
        public uint ack;
    }

    public class UnreliableConnectionHistory<T>
    {
        // Entry with timestamp
        private readonly struct Entry
        {
            public readonly T value;
            public readonly float time;

            public Entry(T v)
            {
                value = v;
                time = Time.realtimeSinceStartup;
            }
        }

        private readonly Dictionary<Connection, Dictionary<uint, Entry>> _history = new();
        private readonly Dictionary<Connection, uint> _lastAcked = new();
        private readonly Dictionary<Connection, uint> _lastSeqId = new();
        private readonly List<UnreliableAck> _pendingAcks = new();

        // Tunables
        private const float ExpireAfter = 10f; // seconds
        private const int KeepMin = 32;
        private const float CleanupInterval = 2f; // run cleanup every 2s max

        private float _lastCleanupTime;

        private uint NextSeqId(Connection conn)
        {
            if (!_lastSeqId.TryGetValue(conn, out var seq))
                seq = 0;

            seq++;
            _lastSeqId[conn] = seq;
            return seq;
        }

        private static void TryDispose(in Entry e)
        {
            if (e.value is IDisposable d)
                d.Dispose();
        }

        private void MaybeCleanup(Dictionary<uint, Entry> history)
        {
            float now = Time.realtimeSinceStartup;
            if (now - _lastCleanupTime < CleanupInterval)
                return;

            _lastCleanupTime = now;

            using var toRemove = DisposableList<uint>.Create(history.Count);
            foreach (var kvp in history)
            {
                if (history.Count - toRemove.Count <= KeepMin)
                    break;

                if (now - kvp.Value.time > ExpireAfter)
                    toRemove.Add(kvp.Key);
            }

            foreach (var key in toRemove)
            {
                if (history.Remove(key, out var e))
                    TryDispose(e);
            }
        }

        // Write new version of data
        public void WriteReliable(BitPacker stream, Connection conn, T newValue)
        {
            if (!_history.TryGetValue(conn, out var connHist))
                _history[conn] = connHist = new Dictionary<uint, Entry>();

            uint seqId = NextSeqId(conn);
            uint acked = _lastAcked.GetValueOrDefault(conn);
            T oldValue = default;

            if (acked > 0 && connHist.TryGetValue(acked, out var oldEntry))
                oldValue = oldEntry.value;

            Packer<PackedUInt>.Write(stream, acked);
            DeltaPacker<PackedUInt>.Write(stream, acked, seqId);
            DeltaPacker<T>.Write(stream, oldValue, newValue);

            connHist[seqId] = new Entry(Packer.Copy(newValue));
            MaybeCleanup(connHist);
        }

        // Read new version of data
        public T ReadReliable(BitPacker stream, Connection conn)
        {
            if (!_history.TryGetValue(conn, out var connHist))
                _history[conn] = connHist = new Dictionary<uint, Entry>();

            uint oldAck = Packer<PackedUInt>.Read(stream);
            uint seqId = DeltaPacker<PackedUInt>.Read(stream, oldAck);

            T oldValue = default;
            if (connHist.TryGetValue(oldAck, out var oldEntry))
                oldValue = oldEntry.value;

            T newValue = default;

            DeltaPacker<T>.Read(stream, oldValue, ref newValue);

            connHist[seqId] = new Entry(Packer.Copy(newValue));
            RegisterAck(conn, seqId);
            MaybeCleanup(connHist);
            return newValue;
        }

        // Receive ack packet
        public void ReceiveAcks(Connection conn, BitPacker stream)
        {
            if (!Packer<bool>.Read(stream))
                return;

            uint ackSeqId = Packer<PackedUInt>.Read(stream).value;
            _lastAcked[conn] = ackSeqId;
        }

        // Send ack packet
        public bool SendAcks(Connection conn, BitPacker stream)
        {
            for (int i = _pendingAcks.Count - 1; i >= 0; i--)
            {
                var pending = _pendingAcks[i];
                if (pending.conn == conn)
                {
                    Packer<bool>.Write(stream, true);
                    Packer<PackedUInt>.Write(stream, pending.ack);
                    _pendingAcks.RemoveAt(i);
                    return true;
                }
            }

            Packer<bool>.Write(stream, false);
            return false;
        }

        private void RegisterAck(Connection conn, uint seqId)
        {
            for (int i = _pendingAcks.Count - 1; i >= 0; i--)
            {
                if (_pendingAcks[i].conn == conn)
                {
                    _pendingAcks[i] = new UnreliableAck { conn = conn, ack = seqId };
                    return;
                }
            }

            _pendingAcks.Add(new UnreliableAck { conn = conn, ack = seqId });
        }

        public void Clear(Connection conn)
        {
            if (_history.Remove(conn, out var dict))
            {
                foreach (var e in dict.Values)
                    TryDispose(e);
            }

            _lastSeqId.Remove(conn);
            _lastAcked.Remove(conn);
        }
    }
}
