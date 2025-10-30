using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public string scene;
    public int height = 8;
    public int width = 8;
    public int[] nullTiles;
    public float[] tileHeights;

    // These vars hold the top left and bottom right coords of where pieces will spawn

    // startingWhiteTiles[0] >= 7, startingWhiteTiles[1] >= 1
    public int[] startingWhiteTiles;
    public int[] startingBlackTiles;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapData))]

public class MapDataEditor : Editor
{
    private MapData mapData;
    public int width = 8;
    public int height = 8;

    public void OnEnable()
    {
        mapData = (MapData)target;
        width = mapData.width;
        height = mapData.height;

        // mapData.nullTiles = new bool[height * width];
        // mapData.tileHeights = new int[height * width];

        // EditorUtility.SetDirty(mapData);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // mapData = (MapData)target;

        // width = mapData.width;
        // height = mapData.height;

        if (mapData.nullTiles == null)
        {
            mapData.nullTiles = new int[width * height];
        }

        if (mapData.tileHeights == null)
        {
            mapData.tileHeights = new float[width * height];
        }

        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset grid sizes", GUILayout.Width(100), GUILayout.Height(50)))
        {
            width = mapData.width;
            height = mapData.height;

            mapData.nullTiles = new int[height * width];
            mapData.tileHeights = new float[height * width];

            for (int i = 0; i < mapData.nullTiles.Length; i++)
            {
                mapData.nullTiles[i] = 1;
                mapData.tileHeights[i] = 0;
            }

            EditorUtility.SetDirty(mapData);
        }

        EditorGUILayout.LabelField("Null tiles");

        if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(25)))
        {
            for (int i = 0; i < mapData.nullTiles.Length; i++)
            {
                mapData.nullTiles[i] = 1;
            }

            EditorUtility.SetDirty(mapData);
        }

        int tileOn = 0;

        for (int j = 0; j < height; j++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < width; i++)
            {
                GUIStyle button = new GUIStyle(GUI.skin.button);
                string buttonText = "T";

                if (mapData.nullTiles[tileOn] == 1)
                {
                    button.normal.textColor = Color.green;
                }
                else if (mapData.nullTiles[tileOn] == 2)
                {
                    button.normal.textColor = Color.black;
                    buttonText = "N";
                }
                else if (mapData.nullTiles[tileOn] == 3)
                {
                    button.normal.textColor = Color.yellow;
                    buttonText = "O";
                }

                if (GUILayout.Button(buttonText, button))
                {
                    if (Event.current.button == 0)
                    {
                        mapData.nullTiles[tileOn]++;

                        if (mapData.nullTiles[tileOn] > 3)
                        {
                            mapData.nullTiles[tileOn] = 1;
                        }
                    }
                    else if (Event.current.button == 1)
                    {
                        mapData.nullTiles[tileOn]--;

                        if (mapData.nullTiles[tileOn] < 1)
                        {
                            mapData.nullTiles[tileOn] = 3;
                        }
                    }
                    

                    Debug.Log(mapData.nullTiles[tileOn]);

                    EditorUtility.SetDirty(mapData);
                }

                tileOn++;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Tile Heights");

        if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(25)))
        {
            for (int i = 0; i < mapData.nullTiles.Length; i++)
            {
                mapData.tileHeights[i] = 0;
            }

            EditorUtility.SetDirty(mapData);
        }

        tileOn = 0;

        for (int j = 0; j < height; j++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < width; i++)
            {
                GUIStyle button = new GUIStyle(GUI.skin.button);

                button.normal.textColor = Color.white;

                if (GUILayout.Button(mapData.tileHeights[tileOn] + "", button))
                {
                    if (Event.current.button == 0)
                    {
                        mapData.tileHeights[tileOn] += 0.1f;
                    }
                    else if (Event.current.button == 1)
                    {
                        mapData.tileHeights[tileOn] -= 0.1f;
                    }

                    EditorUtility.SetDirty(mapData);
                }

                tileOn++;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(mapData);
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif