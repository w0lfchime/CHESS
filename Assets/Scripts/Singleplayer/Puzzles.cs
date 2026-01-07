using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;

public class PieceBoardData
{
    public ChessPieceData data;
    public Vector2Int pos;
    public Team team;
}

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject
{
    // format: PieceId x y, PieceId x y...
    // format: startX startY endX endY, ...
    public List<string> blackTeamMovements;

    // format: PieceId endX endY sequenceOld sequenceNew, ...
    public List<string> whiteTeamMovements;
    public MapData mapData;
    public ChessPieceData PieceToAdd;
    public PieceBoardData[] teamSpawning;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Puzzles))]
public class PuzzlesEditor : Editor
{
    private MapData mapData;
    private Puzzles puzzleData;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        puzzleData = (Puzzles)target;
        mapData = puzzleData.mapData;

        if (mapData == null)
            return;

        if(GUILayout.Button("Reset Tiles", GUILayout.Width(100), GUILayout.Height(25))) System.Array.Clear(puzzleData.teamSpawning, 0, puzzleData.teamSpawning.Length);

        if (puzzleData.teamSpawning == null ||
            puzzleData.teamSpawning.Length != mapData.width * mapData.height)
        {
            puzzleData.teamSpawning = new PieceBoardData[mapData.width * mapData.height];
        }
        if(mapData==null) return;
        int width = mapData.width;
        int height = mapData.height;

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
                
                int index = j * width + i;


                if (Event.current.type == EventType.MouseDown &&
                    rect.Contains(Event.current.mousePosition))
                {
                    if(puzzleData.teamSpawning[index]==null){
                        
                        if (puzzleData.PieceToAdd != null)
                        {
                            puzzleData.teamSpawning[index] = new PieceBoardData
                            {
                                data = puzzleData.PieceToAdd,
                                pos = new Vector2Int(i, j),
                                team = Event.current.button == 0 ? Team.White : Team.Black
                            };

                            Event.current.Use();
                        }
                    }
                    else
                    {
                        puzzleData.teamSpawning[index] = null;
                        Event.current.Use();
                    }
                }

                var piece = puzzleData.teamSpawning[index];
                if (piece != null && piece.data?.image != null)
                {
                    
                    Color oldColor = GUI.color;
                    GUI.color = piece.team == Team.White ? Color.white : Color.gray/2;
                    Rect spriteRect = new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8);
                    GUI.DrawTexture(spriteRect, piece.data.image.texture, ScaleMode.ScaleToFit, true);
                    GUI.color = oldColor;
                }

                

                tileOn++;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif