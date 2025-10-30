using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public string scene;
    public int height = 8;
    public int width = 8;
    [HideInInspector]
    public int[] nullTiles;
    [HideInInspector]
    public float[] tileHeights;
    [HideInInspector]

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

    float setHeight;
    bool writeHeights;

    List<Vector2Int> dragged = new List<Vector2Int>();
    bool mouseDown = false;

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

        if (width != mapData.width || height != mapData.height)
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

        if (GUILayout.Button("Reset Tiles", GUILayout.Width(100), GUILayout.Height(25)))
        {
            for (int i = 0; i < mapData.nullTiles.Length; i++)
            {
                mapData.nullTiles[i] = 1;
            }

            EditorUtility.SetDirty(mapData);
        }

        if (GUILayout.Button("Reset Heights", GUILayout.Width(100), GUILayout.Height(25)))
        {
            for (int i = 0; i < mapData.nullTiles.Length; i++)
            {
                mapData.tileHeights[i] = 0;
            }

            EditorUtility.SetDirty(mapData);
        }

        setHeight = EditorGUILayout.FloatField(
	        "Set Height",
            setHeight
           );

        writeHeights = EditorGUILayout.Toggle("Write Heights", writeHeights);

        int tileOn = 0;
        int iterateColorValue = 1;
        for (int j = 0; j < height; j++)
        {
            if(width%2==0) iterateColorValue++;
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < width; i++)
            {
                iterateColorValue++;

                Rect rect = GUILayoutUtility.GetRect(30, 30, GUILayout.ExpandWidth(false)); // reserve space

                GUIStyle button = new GUIStyle();

                button.normal.textColor = Color.white;
                button.alignment = TextAnchor.MiddleCenter;
                button.fontStyle = FontStyle.Bold;

                if (mapData.nullTiles[tileOn] == 1)
                {
                    if (iterateColorValue % 2 == 1)
                    {
                        GUI.color = new Color(0.255f, 0.255f, 0.255f, 1.000f);
                    }
                    else
                    {
                        GUI.color = new Color(0.294f, 0.294f, 0.294f, 1.000f);
                    }

                }
                else if (mapData.nullTiles[tileOn] == 2)
                {
                    GUI.color = new Color(0.000f, 0.000f, 0.000f, 0.000f);
                }
                else if (mapData.nullTiles[tileOn] == 3)
                {
                    if (iterateColorValue % 2 == 1)
                    {
                        GUI.color = new Color(0.486f, 0.259f, 0.000f, 1.000f);
                    }
                    else
                    {
                        GUI.color = new Color(0.788f, 0.353f, 0.000f, 1.000f);
                    }
                }

                EditorGUI.DrawRect(rect, GUI.color);
                if(Event.current.type == EventType.MouseUp){
                    mouseDown = false;
                }
                if(Event.current.type == EventType.MouseDown){
                    mouseDown = true;
                    dragged.Clear();
                }
                GUI.Button(rect, (mapData.tileHeights[tileOn]==0) ? "" : mapData.tileHeights[tileOn] + "", button);
                if ((mouseDown && rect.Contains(Event.current.mousePosition) && !dragged.Contains(new Vector2Int(j, i))))
                {
                    if (Event.current.button == 1)
                    {
                        if(writeHeights){
                            dragged.Add(new Vector2Int(j, i));
                            if(mapData.tileHeights[tileOn] > 0)
                            {
                                mapData.tileHeights[tileOn] = 0;
                            }else{
                                mapData.tileHeights[tileOn] = setHeight;
                            }
                        }
                        else
                        {
                            dragged.Add(new Vector2Int(j, i));
                            mapData.nullTiles[tileOn] = (mapData.nullTiles[tileOn] == 3) ? 1 : 3;

                            if (mapData.nullTiles[tileOn] < 1)
                            {
                                mapData.nullTiles[tileOn] = 3;
                            }
                        }
                    }
                    else if (Event.current.button == 0)
                    {
                        dragged.Add(new Vector2Int(j, i));
                        mapData.nullTiles[tileOn] = (mapData.nullTiles[tileOn] == 2) ? 1 : 2;

                        if (mapData.nullTiles[tileOn] < 1)
                        {
                            mapData.nullTiles[tileOn] = 3;
                        }
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