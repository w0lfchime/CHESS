using System;

namespace PurrNet.Modules
{
    public struct DeltaMessage : IDisposable
    {
        public SceneID scene;
        public int messageId;
        public int? deltaWith;
        public DeltaValue key;
        public DeltaValue value;

        public void Dispose()
        {
            key.Dispose();
            value.Dispose();
        }
    }
}
