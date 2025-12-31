using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    public static class GitHelper
    {
        static bool? _installed;

        static bool installed
        {
            get
            {
                _installed ??= IsGitInstalled();
                return _installed.Value;
            }
        }

        public static bool CheckGit()
        {
            if (!installed)
            {
                if (EditorUtility.DisplayDialog("Git not installed",
                        "Git is not installed on this machine.\n" +
                        "Please install it, restart UnityHub then try again.", "Install", "Cancel"))
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsEditor:
                            Application.OpenURL("https://git-scm.com/download/win");
                            break;
                        case RuntimePlatform.OSXEditor:
                            Application.OpenURL("https://git-scm.com/download/mac");
                            break;
                        case RuntimePlatform.LinuxEditor:
                            Application.OpenURL("https://git-scm.com/download/linux");
                            break;
                        default:
                            Application.OpenURL("https://git-scm.com/downloads");
                            break;
                    }
                }
                return false;
            }

            return true;
        }

        static bool IsGitInstalled()
        {
            return ToolChecker.CheckTool("git");
        }
    }
}
