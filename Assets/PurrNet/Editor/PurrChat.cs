using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    public class PurrChat : MonoBehaviour
    {
        [MenuItem("Tools/PurrNet/PurrChat AI (Community)", false, -100)]
        public static void OpenPurrChat()
        {
            Application.OpenURL("https://www.purrchat.app/chat");
        }
    }
}
