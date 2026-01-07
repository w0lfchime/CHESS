using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PieceBoardData
{
    public ChessPieceData data;
    public Vector2Int pos;
    public Team team;
}

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject
{
    public string[] blackTeamMovements; // 1 path per layer (4 numbers)
    public string[] whiteTeamMovements; // multiple paths per layer (4 numbers each)
    public MapData mapData;
    public ChessPieceData PieceToAdd;
    public PieceBoardData[] teamSpawning;
    public int layerCount = 1;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Puzzles))]
public class PuzzlesEditor : Editor
{
    private Puzzles puzzleData;
    private MapData mapData;
    private Vector2Int selectedPoint = -Vector2Int.one;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        puzzleData = (Puzzles)target;
        mapData = puzzleData.mapData;

        if (mapData == null) return;

        if (GUILayout.Button("Reset Tiles", GUILayout.Width(100), GUILayout.Height(25)))
        {
            Undo.RecordObject(puzzleData, "Reset Puzzle");
            puzzleData.teamSpawning = new PieceBoardData[mapData.width * mapData.height];
            puzzleData.blackTeamMovements = new string[puzzleData.layerCount - 1];
            puzzleData.whiteTeamMovements = new string[puzzleData.layerCount - 1];
            EditorUtility.SetDirty(puzzleData);
        }

        if (puzzleData.blackTeamMovements.Length != puzzleData.layerCount - 1)
        {
            Array.Resize(ref puzzleData.blackTeamMovements, puzzleData.layerCount - 1);
            Array.Resize(ref puzzleData.whiteTeamMovements, puzzleData.layerCount - 1);
        }

        if (puzzleData.teamSpawning == null ||
            puzzleData.teamSpawning.Length != mapData.width * mapData.height)
        {
            puzzleData.teamSpawning = new PieceBoardData[mapData.width * mapData.height];
        }

        int width = mapData.width;
        int height = mapData.height;

        for (int layer = 0; layer < puzzleData.layerCount; layer++)
        {
            GUILayout.Space(10);

            int tileOn = 0;
            int iterateColorValue = 1;

            for (int y = 0; y < height; y++)
            {
                if (width % 2 == 0) iterateColorValue++;
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < width; x++)
                {
                    iterateColorValue++;

                    Rect rect = GUILayoutUtility.GetRect(30, 30, GUILayout.ExpandWidth(false));

                    Color tileColor = Color.clear;
                    if (mapData.nullTiles[tileOn] == 1)
                        tileColor = (iterateColorValue % 2 == 1)
                            ? new Color(.255f, .255f, .255f)
                            : new Color(.294f, .294f, .294f);
                    else if (mapData.nullTiles[tileOn] == 3)
                        tileColor = (iterateColorValue % 2 == 1)
                            ? new Color(.486f, .259f, 0f)
                            : new Color(.788f, .353f, 0f);

                    EditorGUI.DrawRect(rect, tileColor);

                    int index = x + y * width;

                    PieceBoardData[] temp = (PieceBoardData[])puzzleData.teamSpawning.Clone();

                    // Apply previous layers
                    for (int l = 0; l < layer - 1; l++)
                    {
                        if (!string.IsNullOrEmpty(puzzleData.blackTeamMovements[l]))
                        {
                            var b = puzzleData.blackTeamMovements[l].Split(' ');
                            if (b.Length == 4)
                            {
                                int i1 = int.Parse(b[0]) + int.Parse(b[1]) * width;
                                int i2 = int.Parse(b[2]) + int.Parse(b[3]) * width;
                                temp[i2] = temp[i1];
                                temp[i1] = null;
                            }
                        }

                        if (!string.IsNullOrEmpty(puzzleData.whiteTeamMovements[l]))
                        {
                            var w = puzzleData.whiteTeamMovements[l].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i + 3 < w.Length; i += 4)
                            {
                                int i1 = int.Parse(w[i]) + int.Parse(w[i + 1]) * width;
                                int i2 = int.Parse(w[i + 2]) + int.Parse(w[i + 3]) * width;
                                temp[i2] = temp[i1];
                                temp[i1] = null;
                            }
                        }
                    }

                    Event e = Event.current;
                    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                    {
                        Undo.RecordObject(puzzleData, "Puzzle Edit");

                        if (e.button == 0 || e.button == 1)
                        {
                            if (puzzleData.teamSpawning[index]?.data == null && puzzleData.PieceToAdd != null)
                            {
                                puzzleData.teamSpawning[index] = new PieceBoardData
                                {
                                    data = puzzleData.PieceToAdd,
                                    pos = new Vector2Int(x, y),
                                    team = e.button == 0 ? Team.White : Team.Black
                                };
                            }
                            else
                            {
                                puzzleData.teamSpawning[index] = null;
                            }

                            EditorUtility.SetDirty(puzzleData);
                            e.Use();
                        }
                        else if (e.button == 2)
                        {
                            if (selectedPoint != -Vector2Int.one)
                            {
                                int fromIndex = selectedPoint.x + selectedPoint.y * width;
                                var piece = temp[fromIndex];

                                if (piece != null)
                                {
                                    string move = $"{selectedPoint.x} {selectedPoint.y} {x} {y}";

                                    if (piece.team == Team.Black)
                                    {
                                        puzzleData.blackTeamMovements[layer] = move;
                                    }
                                    else if (piece.team == Team.White)
                                    {
                                        puzzleData.whiteTeamMovements[layer] += move + " ";
                                    }
                                }

                                selectedPoint = -Vector2Int.one;
                                EditorUtility.SetDirty(puzzleData);
                            }
                            else if (temp[index] != null)
                            {
                                selectedPoint = new Vector2Int(x, y);
                            }

                            e.Use();
                        }
                    }

                    var p = temp[index];
                    if (p != null && p.data?.image != null)
                    {
                        Color old = GUI.color;
                        GUI.color = p.team == Team.White ? Color.white : Color.gray * 0.5f;
                        GUI.DrawTexture(
                            new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8),
                            p.data.image.texture,
                            ScaleMode.ScaleToFit,
                            true
                        );
                        GUI.color = old;
                    }

                    tileOn++;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif
