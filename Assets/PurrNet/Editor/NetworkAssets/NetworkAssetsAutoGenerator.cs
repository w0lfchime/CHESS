#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    public class NetworkAssetsPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var all = AssetDatabase.FindAssets("t:NetworkAssets")
                .Select(guid => AssetDatabase.LoadAssetAtPath<NetworkAssets>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(n => n && n.autoGenerate && n.folder)
                .ToArray();

            foreach (var netAsset in all)
            {
                string folderPath = AssetDatabase.GetAssetPath(netAsset.folder);
                bool relevantChange = importedAssets.Any(p => p.StartsWith(folderPath)) ||
                                      deletedAssets.Any(p => p.StartsWith(folderPath)) ||
                                      movedAssets.Any(p => p.StartsWith(folderPath)) ||
                                      movedFromAssetPaths.Any(p => p.StartsWith(folderPath));

                if (relevantChange)
                    netAsset.GenerateAssets();
            }
        }
    }
}
#endif