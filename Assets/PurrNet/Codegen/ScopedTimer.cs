#if UNITY_MONO_CECIL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace PurrNet.Codegen
{
    public readonly struct ScopedTimer : IDisposable
    {
        private readonly string _message;
        private readonly Stopwatch _stopwatch;
        private readonly List<DiagnosticMessage> _messages;

        /// <summary>
        /// Starts a new scoped timer.
        /// </summary>
        public ScopedTimer(string message, List<DiagnosticMessage> list)
        {
            _message = message;
            _messages = list ?? throw new ArgumentNullException(nameof(list));
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Stops the timer and logs the elapsed time.
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
            _messages.Add(new DiagnosticMessage
            {
                DiagnosticType = DiagnosticType.Warning,
                MessageData = $"[{_message}] took {_stopwatch.ElapsedMilliseconds} ms."
            });
        }
    }
}
#endif