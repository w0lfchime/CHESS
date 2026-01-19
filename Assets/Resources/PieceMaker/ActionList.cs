using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New ActionList", menuName = "Chess/Action List")]
public class ActionList : ScriptableObject
{
    public Action[] Actions;

#if UNITY_EDITOR
    // When the ActionList asset is edited in the inspector, propagate changes
    // (traits and TileColor) to all ChessPieceData assets that reference actions
    // with matching names so pieces pick up edits immediately.
    // Debounce updates so changing colors in the picker doesn't trigger
    // immediate propagation on every drag event. We wait until the user
    // stops interacting for a short period before applying TileColor.
    private static ActionList s_pendingInstance;
    private static double s_lastValidateTime;
    private static bool s_updateScheduled = false;
    private const double s_debounceSeconds = 0.25;

    private void OnValidate()
    {
        if (Actions == null || Actions.Length == 0)
            return;

        s_pendingInstance = this;
        s_lastValidateTime = EditorApplication.timeSinceStartup;

        if (!s_updateScheduled)
        {
            s_updateScheduled = true;
            EditorApplication.update += DebouncedUpdate;
        }
    }

    private static void DebouncedUpdate()
    {
        if (EditorApplication.timeSinceStartup - s_lastValidateTime < s_debounceSeconds)
            return;

        EditorApplication.update -= DebouncedUpdate;
        s_updateScheduled = false;

        var instance = s_pendingInstance;
        s_pendingInstance = null;
        if (instance == null || instance.Actions == null || instance.Actions.Length == 0)
            return;

        string[] guids = AssetDatabase.FindAssets("t:ChessPieceData");
        var dirtyAssets = new System.Collections.Generic.List<ChessPieceData>();
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            ChessPieceData data = AssetDatabase.LoadAssetAtPath<ChessPieceData>(path);
            if (data == null || data.abilities == null)
                continue;

            bool dirty = false;

            foreach (var ability in data.abilities)
            {
                if (ability == null || ability.actions == null)
                    continue;

                for (int i = 0; i < ability.actions.Count; i++)
                {
                    var a = ability.actions[i];
                    if (a == null || string.IsNullOrEmpty(a.name))
                        continue;

                    int idx = System.Array.FindIndex(instance.Actions, x => x != null && x.name == a.name);
                    if (idx >= 0)
                    {
                        var src = instance.Actions[idx];
                        if (src != null)
                        {
                            a.traits = src.traits;
                            a.TileColor = src.TileColor;
                            dirty = true;
                        }
                    }
                }
            }

            if (dirty)
            {
                dirtyAssets.Add(data);
            }
        }

        if (dirtyAssets.Count > 0)
        {
            var toSave = dirtyAssets.ToArray();
            EditorApplication.delayCall += () =>
            {
                foreach (var d in toSave)
                {
                    if (d != null)
                        EditorUtility.SetDirty(d);
                }
                AssetDatabase.SaveAssets();
            };
        }
    }
#endif
}
