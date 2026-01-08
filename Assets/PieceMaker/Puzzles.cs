using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

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
    public ChessPieceData unlockData;
    public string[] blackTeamMovements;

    public string[] whiteTeamMovements;
    public MapData mapData;
    public ChessPieceData PieceToAdd;
    [HideInInspector]
    public PieceBoardData[] teamSpawning;
    public int layerCount = 1;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Puzzles))]
public class PuzzlesEditor : Editor
{
    private MapData mapData;
    private Puzzles puzzleData;
    private Vector2Int selectedPoint = -Vector2Int.one;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        puzzleData = (Puzzles)target;
        mapData = puzzleData.mapData;

        if (mapData == null)
            return;

        if (GUILayout.Button("Reset Tiles", GUILayout.Width(100), GUILayout.Height(25)))
        {
            Undo.RecordObject(puzzleData, "Reset Puzzle");
            puzzleData.teamSpawning = new PieceBoardData[mapData.width * mapData.height];

            int moveLayers = Mathf.Max(0, puzzleData.layerCount - 1);
            puzzleData.blackTeamMovements = new string[moveLayers];
            puzzleData.whiteTeamMovements = new string[moveLayers];

            for (int i = 0; i < moveLayers; i++)
            {
                puzzleData.blackTeamMovements[i] = "";
                puzzleData.whiteTeamMovements[i] = "";
            }
        }

        int requiredMoveLayers = Mathf.Max(0, puzzleData.layerCount - 1);

        if (puzzleData.blackTeamMovements == null) puzzleData.blackTeamMovements = new string[requiredMoveLayers];
        if (puzzleData.whiteTeamMovements == null) puzzleData.whiteTeamMovements = new string[requiredMoveLayers];

        if (puzzleData.blackTeamMovements.Length != requiredMoveLayers)
        {
            Array.Resize(ref puzzleData.blackTeamMovements, requiredMoveLayers);
            Array.Resize(ref puzzleData.whiteTeamMovements, requiredMoveLayers);
        }

        for (int i = 0; i < requiredMoveLayers; i++)
        {
            if (puzzleData.blackTeamMovements[i] == null) puzzleData.blackTeamMovements[i] = "";
            if (puzzleData.whiteTeamMovements[i] == null) puzzleData.whiteTeamMovements[i] = "";
        }

        if (puzzleData.teamSpawning == null || puzzleData.teamSpawning.Length != mapData.width * mapData.height)
        {
            puzzleData.teamSpawning = new PieceBoardData[mapData.width * mapData.height];
        }

        int width = mapData.width;
        int height = mapData.height;

        // --- helpers (kept inside method for minimal changes to file structure) ---
        int ToIndex(int x, int y) => x + y * width;

        static bool TryParseMove4(string s, out int sx, out int sy, out int ex, out int ey)
        {
            sx = sy = ex = ey = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length < 4) return false;
            return int.TryParse(t[0], out sx) && int.TryParse(t[1], out sy) && int.TryParse(t[2], out ex) && int.TryParse(t[3], out ey);
        }

        static List<int> ParseWhiteMoveQuads(string s)
        {
            // returns tokens list in multiples of 4: sx sy ex ey ...
            var list = new List<int>();
            if (string.IsNullOrWhiteSpace(s)) return list;

            var t = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 3 < t.Length; i += 4)
            {
                if (int.TryParse(t[i + 0], out int sx) &&
                    int.TryParse(t[i + 1], out int sy) &&
                    int.TryParse(t[i + 2], out int ex) &&
                    int.TryParse(t[i + 3], out int ey))
                {
                    list.Add(sx); list.Add(sy); list.Add(ex); list.Add(ey);
                }
            }
            return list;
        }

        void RemoveAllMovesRelatedTo(int x, int y)
        {
            int moveLayers = Mathf.Max(0, puzzleData.layerCount - 1);
            for (int l = 0; l < moveLayers; l++)
            {
                // black: single move per layer
                if (TryParseMove4(puzzleData.blackTeamMovements[l], out int bsx, out int bsy, out int bex, out int bey))
                {
                    if ((bsx == x && bsy == y) || (bex == x && bey == y))
                        puzzleData.blackTeamMovements[l] = "";
                }

                // white: many quads per layer
                var w = ParseWhiteMoveQuads(puzzleData.whiteTeamMovements[l]);
                if (w.Count == 0) continue;

                var rebuilt = new List<int>(w.Count);
                for (int i = 0; i < w.Count; i += 4)
                {
                    int wsx = w[i + 0], wsy = w[i + 1], wex = w[i + 2], wey = w[i + 3];
                    bool related = (wsx == x && wsy == y) || (wex == x && wey == y);
                    if (!related)
                    {
                        rebuilt.Add(wsx); rebuilt.Add(wsy); rebuilt.Add(wex); rebuilt.Add(wey);
                    }
                }

                puzzleData.whiteTeamMovements[l] = rebuilt.Count == 0 ? "" : string.Join(" ", rebuilt) + " ";
            }
        }

        void RemoveMovesInLayerRelatedTo(int moveLayerIndex, int x, int y)
        {
            if (moveLayerIndex < 0 || moveLayerIndex >= Mathf.Max(0, puzzleData.layerCount - 1)) return;

            // black
            if (TryParseMove4(puzzleData.blackTeamMovements[moveLayerIndex], out int bsx, out int bsy, out int bex, out int bey))
            {
                if ((bsx == x && bsy == y) || (bex == x && bey == y))
                    puzzleData.blackTeamMovements[moveLayerIndex] = "";
            }

            // white
            var w = ParseWhiteMoveQuads(puzzleData.whiteTeamMovements[moveLayerIndex]);
            if (w.Count > 0)
            {
                var rebuilt = new List<int>(w.Count);
                for (int i = 0; i < w.Count; i += 4)
                {
                    int wsx = w[i + 0], wsy = w[i + 1], wex = w[i + 2], wey = w[i + 3];
                    bool related = (wsx == x && wsy == y) || (wex == x && wey == y);
                    if (!related)
                    {
                        rebuilt.Add(wsx); rebuilt.Add(wsy); rebuilt.Add(wex); rebuilt.Add(wey);
                    }
                }
                puzzleData.whiteTeamMovements[moveLayerIndex] = rebuilt.Count == 0 ? "" : string.Join(" ", rebuilt) + " ";
            }
        }

        PieceBoardData CopyPiece(PieceBoardData src, int newX, int newY)
        {
            if (src == null) return null;
            return new PieceBoardData
            {
                data = src.data,
                team = src.team,
                pos = new Vector2Int(newX, newY)
            };
        }

        PieceBoardData[] ApplyLayerMoves(PieceBoardData[] baseBoard, int moveLayerIndex)
        {
            var board = (PieceBoardData[])baseBoard.Clone();

            if (moveLayerIndex >= 0 && moveLayerIndex < puzzleData.blackTeamMovements.Length)
            {
                if (TryParseMove4(puzzleData.blackTeamMovements[moveLayerIndex], out int sx, out int sy, out int ex, out int ey))
                {
                    int sIdx = ToIndex(sx, sy);
                    int eIdx = ToIndex(ex, ey);
                    var src = board[sIdx];
                    if (src != null && src.team == Team.Black)
                    {
                        board[eIdx] = CopyPiece(src, ex, ey);
                        board[sIdx] = null;
                    }
                }
            }

            if (moveLayerIndex >= 0 && moveLayerIndex < puzzleData.whiteTeamMovements.Length)
            {
                var tokens = ParseWhiteMoveQuads(puzzleData.whiteTeamMovements[moveLayerIndex]);
                if (tokens.Count >= 4)
                {
                    var snapshot = (PieceBoardData[])board.Clone();
                    var startsToRemove = new HashSet<int>();

                    for (int i = 0; i < tokens.Count; i += 4)
                    {
                        int sx = tokens[i + 0], sy = tokens[i + 1], ex = tokens[i + 2], ey = tokens[i + 3];
                        int sIdx = ToIndex(sx, sy);
                        int eIdx = ToIndex(ex, ey);

                        var src = snapshot[sIdx];
                        if (src != null && src.team == Team.White)
                        {
                            board[eIdx] = CopyPiece(src, ex, ey);
                            startsToRemove.Add(sIdx);
                        }
                    }

                    foreach (var sIdx in startsToRemove)
                        board[sIdx] = null;
                }
            }

            return board;
        }

        PieceBoardData[] BuildBoardForLayer(int layer)
        {
            var board = (PieceBoardData[])puzzleData.teamSpawning.Clone();
            for (int m = 0; m < layer; m++)
            {
                board = ApplyLayerMoves(board, m);
            }
            return board;
        }

        for (int layer = 0; layer < puzzleData.layerCount; layer++)
        {
            GUILayout.Space(10);

            var centers = new Dictionary<int, Vector2>(width * height);

            var tempTeamSpawning = BuildBoardForLayer(layer);

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
                    centers[ToIndex(x, y)] = rect.center;

                    GUIStyle button = new GUIStyle
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold
                    };
                    button.normal.textColor = Color.white;

                    if (mapData.nullTiles[tileOn] == 1)
                    {
                        GUI.color = (iterateColorValue % 2 == 1)
                            ? new Color(0.255f, 0.255f, 0.255f, 1.000f)
                            : new Color(0.294f, 0.294f, 0.294f, 1.000f);
                    }
                    else if (mapData.nullTiles[tileOn] == 2)
                    {
                        GUI.color = new Color(0.000f, 0.000f, 0.000f, 0.000f);
                    }
                    else if (mapData.nullTiles[tileOn] == 3)
                    {
                        GUI.color = (iterateColorValue % 2 == 1)
                            ? new Color(0.486f, 0.259f, 0.000f, 1.000f)
                            : new Color(0.788f, 0.353f, 0.000f, 1.000f);
                    }

                    EditorGUI.DrawRect(rect, GUI.color);

                    int index = ToIndex(x, y);
                    Event e = Event.current;

                    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                    {
                        Undo.RecordObject(puzzleData, "Puzzle Edit");

                        // LAYER 0: allow add/remove pieces
                        if (layer == 0)
                        {
                            if (Event.current.button == 0 || Event.current.button == 1)
                            {
                                var existing = puzzleData.teamSpawning[index];

                                if (existing == null || existing.data == null)
                                {
                                    if (puzzleData.PieceToAdd != null)
                                    {
                                        puzzleData.teamSpawning[index] = new PieceBoardData
                                        {
                                            data = puzzleData.PieceToAdd,
                                            pos = new Vector2Int(x, y),
                                            team = Event.current.button == 0 ? Team.White : Team.Black
                                        };
                                    }
                                }
                                else
                                {
                                    puzzleData.teamSpawning[index] = null;
                                    RemoveAllMovesRelatedTo(x, y);
                                }

                                EditorUtility.SetDirty(puzzleData);
                                Event.current.Use();
                            }
                            else if (Event.current.button == 2)
                            {
                                if (selectedPoint != -Vector2Int.one)
                                {
                                    if (selectedPoint.x != x || selectedPoint.y != y)
                                    {
                                        int selIdx = ToIndex(selectedPoint.x, selectedPoint.y);
                                        var selPiece = tempTeamSpawning[selIdx];

                                        if (selPiece != null && selPiece.data != null)
                                        {
                                            int moveLayerIndex = layer - 1;
                                            moveLayerIndex = 0;

                                            if (selPiece.team == Team.Black)
                                            {
                                                puzzleData.blackTeamMovements[moveLayerIndex] =
                                                    $"{selectedPoint.x} {selectedPoint.y} {x} {y}";
                                            }
                                            else if (selPiece.team == Team.White)
                                            {
                                                puzzleData.whiteTeamMovements[moveLayerIndex] +=
                                                    $"{selectedPoint.x} {selectedPoint.y} {x} {y} ";
                                            }

                                            EditorUtility.SetDirty(puzzleData);
                                        }
                                    }

                                    selectedPoint = -Vector2Int.one;
                                }
                                else
                                {
                                    if (tempTeamSpawning[index] != null && tempTeamSpawning[index].data != null)
                                    {
                                        selectedPoint = new Vector2Int(x, y);
                                    }
                                }

                                Event.current.Use();
                            }
                        }
                        else
                        {
                            if (Event.current.button == 0 || Event.current.button == 1)
                            {
                                RemoveMovesInLayerRelatedTo(layer - 1, x, y);

                                if (selectedPoint != -Vector2Int.one)
                                    selectedPoint = -Vector2Int.one;

                                EditorUtility.SetDirty(puzzleData);
                                Event.current.Use();
                            }
                            else if (Event.current.button == 2)
                            {
                                int moveLayerIndex = layer;
                                if (moveLayerIndex >= puzzleData.layerCount - 1)
                                {
                                    selectedPoint = -Vector2Int.one;
                                    Event.current.Use();
                                    goto DrawPiecesAndContinue;
                                }

                                if (selectedPoint != -Vector2Int.one)
                                {
                                    if (selectedPoint.x != x || selectedPoint.y != y)
                                    {
                                        int selIdx = ToIndex(selectedPoint.x, selectedPoint.y);
                                        var selPiece = tempTeamSpawning[selIdx];

                                        if (selPiece != null && selPiece.data != null)
                                        {
                                            if (selPiece.team == Team.Black)
                                            {
                                                puzzleData.blackTeamMovements[moveLayerIndex] =
                                                    $"{selectedPoint.x} {selectedPoint.y} {x} {y}";
                                            }
                                            else if (selPiece.team == Team.White)
                                            {
                                                puzzleData.whiteTeamMovements[moveLayerIndex] +=
                                                    $"{selectedPoint.x} {selectedPoint.y} {x} {y} ";
                                            }

                                            EditorUtility.SetDirty(puzzleData);
                                        }
                                    }

                                    selectedPoint = -Vector2Int.one;
                                }
                                else
                                {
                                    if (tempTeamSpawning[index] != null && tempTeamSpawning[index].data != null)
                                    {
                                        selectedPoint = new Vector2Int(x, y);
                                    }
                                }

                                Event.current.Use();
                            }
                        }
                    }

                DrawPiecesAndContinue:

                    var piece = tempTeamSpawning[index];
                    if (piece != null && piece.data?.image != null)
                    {
                        Color oldColor = GUI.color;
                        GUI.color = piece.team == Team.White ? Color.white : Color.gray / 2f;
                        Rect spriteRect = new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8);
                        GUI.DrawTexture(spriteRect, piece.data.image.texture, ScaleMode.ScaleToFit, true);
                        GUI.color = oldColor;
                    }

                    tileOn++;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Draw paths for this layer's movement definitions (moves that produce NEXT layer)
            // For layer 0, show moveLayerIndex 0; for layer N, show moveLayerIndex N (if exists).
            int showMoveLayerIndex = layer;
            if (showMoveLayerIndex >= 0 && showMoveLayerIndex < puzzleData.layerCount - 1)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Handles.BeginGUI();

                    if (TryParseMove4(puzzleData.blackTeamMovements[showMoveLayerIndex], out int bsx, out int bsy, out int bex, out int bey))
                    {
                        int sIdx = ToIndex(bsx, bsy);
                        int eIdx = ToIndex(bex, bey);
                        if (centers.TryGetValue(sIdx, out var a) && centers.TryGetValue(eIdx, out var b))
                        {
                            Handles.color = Color.black;
                            Handles.DrawLine(a, b);
                        }
                    }

                    var w = ParseWhiteMoveQuads(puzzleData.whiteTeamMovements[showMoveLayerIndex]);
                    for (int i = 0; i < w.Count; i += 4)
                    {
                        int wsx = w[i + 0], wsy = w[i + 1], wex = w[i + 2], wey = w[i + 3];
                        int sIdx = ToIndex(wsx, wsy);
                        int eIdx = ToIndex(wex, wey);
                        if (centers.TryGetValue(sIdx, out var a) && centers.TryGetValue(eIdx, out var b))
                        {
                            Handles.color = Color.white;
                            Handles.DrawLine(a, b);
                        }
                    }

                    Handles.EndGUI();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
