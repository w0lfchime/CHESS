using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Tutorial : MonoBehaviour
{
    public List<Vector3> positions = new List<Vector3>();
}


#if UNITY_EDITOR

[CustomEditor(typeof(Tutorial))]
public class PositionRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Tutorial recorder = (Tutorial)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Add Current Position"))
        {
            Undo.RecordObject(recorder, "Add Position");
            recorder.positions.Add(recorder.transform.position);
            EditorUtility.SetDirty(recorder);
        }
    }
}
#endif
