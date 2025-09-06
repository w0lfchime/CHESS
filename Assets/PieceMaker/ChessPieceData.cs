using System;
using UnityEditor;
using UnityEngine;

public enum TriggerType
{
    Manual,
    Death,
    OnMove,
    OnAttack
}

[System.Serializable]
public class AbilityGrid
{
    public string abilityName;
    public bool enabled;
    public TriggerType trigger;

    public Wrapper<Elements>[] visualGrid;
    public int[] grid;

    public void Init(int size)
    {
        visualGrid = new Wrapper<Elements>[size];
        for (int i = 0; i < size; i++)
        {
            visualGrid[i] = new Wrapper<Elements>();
            visualGrid[i].values = new Elements[size];
        }
        grid = new int[size * size];
    }
}
 
public enum Elements { CanMove, CantMove }

// Holds visualGridMoves and visualGridTakes data
[CreateAssetMenu(fileName = "New Chess Piece", menuName = "Chess/ChessPiece")]
public class ChessPieceData : ScriptableObject
{
    public int size = 11;
    public Mesh model;
    

    public AbilityGrid[] abilities = new AbilityGrid[]
    {
        new AbilityGrid { abilityName = "moves" },
        new AbilityGrid { abilityName = "takes" },
        new AbilityGrid { abilityName = "friendlyFire" },
        new AbilityGrid { abilityName = "explodes" },
        new AbilityGrid { abilityName = "promotes" },
        new AbilityGrid { abilityName = "firstMove" },
        new AbilityGrid { abilityName = "firstTurnMove" }
    };

    public void SetSize(int setSize){
        size = setSize;
    }
}

 
#if UNITY_EDITOR
[CustomEditor(typeof(ChessPieceData))]
public class ChessPieceDataEditor : Editor
{
    //initialize clipboard for copy/paste
    private string clipboard;
 
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ChessPieceData script = (ChessPieceData)target;

        //extra variables
        script.model = (Mesh)EditorGUILayout.ObjectField(
            "3D Model",
            script.model,
            typeof(Mesh),
            false
        );

        //abilities

        foreach (var ability in script.abilities)
        {
            ability.enabled = GUILayout.Toggle(ability.enabled, ability.abilityName);

            if (ability.enabled)
            {
                // Dropdown for trigger type
                ability.trigger = (TriggerType)EditorGUILayout.EnumPopup("Trigger", ability.trigger);

                GUILayout.Label(ability.abilityName + " Grid", EditorStyles.boldLabel);
                if (ability.visualGrid == null) ability.Init(script.size);

                if (ability.visualGrid == null || ability.visualGrid.Length != script.size || ability.grid == null || ability.grid.Length != script.size * script.size)
                {
                    ability.Init(script.size);
                }

                DrawVisualGrid(script.size, ability.visualGrid, ability.grid);
                if (GUILayout.Button("Reset " + ability.abilityName))
                    ability.Init(script.size);
            }
        }


        GUILayout.Space(10);
        // copy button setup
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy All"))
        {
            clipboard = JsonUtility.ToJson(script, true);
            EditorGUIUtility.systemCopyBuffer = clipboard;
        }
        // paste button setup
        if (GUILayout.Button("Paste All"))
        {
            try
            {
                string json = EditorGUIUtility.systemCopyBuffer;
                JsonUtility.FromJsonOverwrite(json, script);
                EditorUtility.SetDirty(script);
            }
            catch
            {
                Debug.LogWarning("Paste failed: invalid JSON in clipboard.");
            }
        }
        GUILayout.EndHorizontal();

 
        serializedObject.ApplyModifiedProperties();
    }
 
    private void DrawVisualGrid(int size, Wrapper<Elements>[] visualGrid, int[] intGrid)
    {
        try
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < size; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < size; j++)
                {
                    int cellValue = (int)visualGrid[i].values[j];

                    // --- Set button color ---
                    Color oldColor = GUI.backgroundColor;
                    if (i == size / 2 && j == size / 2)
                        GUI.backgroundColor = Color.red;
                    else if (cellValue == 1)
                        GUI.backgroundColor = Color.green;
                    else if (cellValue == 2)
                        GUI.backgroundColor = new Color(1f, 0.5f, 0f); // orange
                    else
                        GUI.backgroundColor = Color.white;

                    // --- Draw button ---
                    if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        if (Event.current.button == 1) // right-click → orange
                        {
                            cellValue = Mathf.Clamp(-cellValue+2, 0, 2);
                            Event.current.Use();
                        }
                        else // left-click → cycle 0 → 1 → 2 → 0
                        {
                            cellValue = Mathf.Clamp(-cellValue+1, 0, 1);
                        }
                    }

                    GUI.backgroundColor = oldColor;

                    // --- Write back to both grids ---
                    visualGrid[i].values[j] = (Elements)cellValue;
                    intGrid[i * size + j] = cellValue;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
#endif

 
[System.Serializable]
public class Wrapper<T>
{
    public T[] values;
}