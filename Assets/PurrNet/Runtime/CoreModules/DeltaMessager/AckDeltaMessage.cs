namespace PurrNet.Modules
{
    public struct AckDeltaMessage
    {
        public SceneID scene;
        public int messageId;
        public DeltaValue key;
    }
}
