namespace PurrNet.Editor
{
    public static class UnityHubHelper
    {
        public static string Path =>
#if UNITY_EDITOR_WIN
            "C:/Program Files/Unity Hub/Unity Hub.exe";
#elif UNITY_EDITOR_OSX
            "/Applications/Unity/Hub.app/Contents/MacOS/Unity/Hub";
#elif UNITY_EDITOR_LINUX
            "~/Applications/Unity/Hub.AppImage";
#endif

        public static string Headless =>
#if UNITY_EDITOR_WIN
            "-- --headless";
#elif UNITY_EDITOR_OSX
            "-- --headless";
#elif UNITY_EDITOR_LINUX
            "--headless";
#endif
    }
}
