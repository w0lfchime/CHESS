using UnityEditor;
using UnityEngine;

namespace PurrNet.Profiler.Deltas.Editor
{
    public static class DeltasWindowCreator
    {
        [MenuItem("Tools/PurrNet/Analysis/Deltas Analyzer")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<DeltasWindow>("Deltas Analyzer");
            var purrnetLogo = Resources.Load("purricon") as Texture2D;
            window.titleContent = new GUIContent("Deltas Analyzer", purrnetLogo, "Deltas Analyzer");
            window.Show();
        }
    }
}
