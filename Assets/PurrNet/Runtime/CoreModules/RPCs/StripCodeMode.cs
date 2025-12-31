namespace PurrNet
{
    public enum StripCodeMode
    {
        DoNotStrip,
        StripAll,
        ReplaceWithEmptyMethod,
        ReplaceWithLogWarning,
        ReplaceWithLogError,
        ThrowNotSupportedException
    }

    public enum StripCodeModeOverride
    {
        Settings,
        DoNotStrip,
        StripAll,
        ReplaceWithEmptyMethod,
        ReplaceWithLogWarning,
        ReplaceWithLogError,
        ThrowNotSupportedException
    }
}
