using System;
using System.Collections.Generic;
using PurrNet.Packing;
using PurrNet.Transports;

namespace PurrNet.Modules
{
    public class ReliableConnectionHistory<T>
    {
        private readonly Dictionary<Connection, T> _lastRead = new ();
        private readonly Dictionary<Connection, T> _lastWritten = new ();

        private T GetLastWrittenReliableId(Connection conn)
        {
            return _lastWritten.GetValueOrDefault(conn);
        }

        private T GetLastReadReliableId(Connection conn)
        {
            return _lastRead.GetValueOrDefault(conn);
        }

        private void UpdateLastRead(Connection conn, T data)
        {
            if (_lastRead.TryGetValue(conn, out var old) && old is IDisposable disposable)
                disposable.Dispose();
            _lastRead[conn] = Packer.Copy(data);
        }

        private void UpdateLastWritten(Connection conn, T data)
        {
            if (_lastWritten.TryGetValue(conn, out var old) && old is IDisposable disposable)
                disposable.Dispose();
            _lastWritten[conn] = Packer.Copy(data);
        }

        public void WriteReliable(BitPacker stream, Connection conn, T newValue)
        {
            var old = GetLastWrittenReliableId(conn);
            DeltaPacker<T>.Write(stream, old, newValue);
            UpdateLastWritten(conn, newValue);
        }

        public T ReadReliable(BitPacker stream, Connection conn)
        {
            var old = GetLastReadReliableId(conn);
            var newValue = default(T);
            DeltaPacker<T>.Read(stream, old, ref newValue);
            UpdateLastRead(conn, newValue);
            return newValue;
        }

        public void Clear(Connection conn)
        {
            if (_lastRead.Remove(conn, out var old) && old is IDisposable disposable)
                disposable.Dispose();

            if (_lastWritten.Remove(conn, out var oldRead) && oldRead is IDisposable read)
                read.Dispose();
        }
    }
}
