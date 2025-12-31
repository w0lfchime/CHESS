using System;

namespace PurrNet
{
    [Flags]
    public enum StartFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// The server should start in the editor.
        /// </summary>
        Editor = 1,

        /// <summary>
        /// The client should start in the editor.
        /// A clone is an editor instance that is not the main editor instance.
        /// For example when you use ParrelSync or other tools that create a clone of the editor.
        /// </summary>
        Clone = 2,

        /// <summary>
        /// A client build.
        /// It is a build that doesn't contain the UNITY_SERVER define.
        /// </summary>
        ClientBuild = 4,

        /// <summary>
        /// A server build.
        /// It is a build that contains the UNITY_SERVER define.
        /// The define is added automatically when doing a server build.
        /// </summary>
        ServerBuild = 8
    }
}