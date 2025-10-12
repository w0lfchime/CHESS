using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//All the action triggers. The chess board activates the triggers based on what is happening in the game.
public enum TriggerType
{
    TurnAction,
    FirstTurnAction,
    FirstActionAction,
    OnDeath,
    OnMove,
    OnTake,
    OnPromote,
    OnTurnSwap
}

//All the action traits. These are low level traits that determine how an action specifically interacts with the tiles around it.
public enum ActionTrait
{
    //conditional traits
    remove_unselected = 0,

    apply_to_empty_space = 2,
    apply_to_ownteam_space = 3,
    apply_to_opposingteam_space = 4,

    command_goto = 5,
    command_pushback = 12,
    remove_obstructed = 6,

    shift_focus = 7,

    //traits that do something
    spawn_water = 8,
    command_killpiece = 9,

    spawn_explosion_effect = 10,

    animate_jump = 11

}


[System.Serializable]
public class Ability
{
    public string name;
    public bool BasicMovement; // basically just for anything that should be shown without having to switch abilities, so multiple abilities and triggers can still be basicmovement
    public TriggerType trigger;
    public List<Action> actions = new List<Action>();
}

[System.Serializable]
public class Action
{
    public string name;

    public ActionTrait[] traits;

    [HideInInspector]
    public Wrapper<Elements>[] visualGrid;

    [HideInInspector]
    public int[] grid;

    public void Init(int gridSize)
    {
        visualGrid = new Wrapper<Elements>[gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            visualGrid[i] = new Wrapper<Elements>();
            visualGrid[i].values = new Elements[gridSize];
        }
        grid = new int[gridSize * gridSize];
    }

    public Action Clone()
    {
        return new Action { name = this.name, traits = this.traits };
    }
}
 
public enum Elements { CanMove, CantMove }

// Holds general chess piece data
[CreateAssetMenu(fileName = "New Chess Piece Data", menuName = "Chess/ChessPieceData")]
public class ChessPieceData : ScriptableObject
{

    public string name = "NoNameSet";
    public Mesh model;
    public float model_scale_multiplier;
    public bool lifeLine;

    public List<ChessPieceData> promotable = new List<ChessPieceData>();


    public int gridSize = 11;

    public List<Ability> abilities = new List<Ability>();

    //All the actions
    public ActionList actionList;

    public void SetSize(int setSize)
    {
        gridSize = setSize;
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

		script.model_scale_multiplier = EditorGUILayout.FloatField(
	        "Model Scale Multiplier",
	        script.model_scale_multiplier
           );


		script.lifeLine = EditorGUILayout.Toggle("LifeLine", script.lifeLine);

        SerializedProperty promotableList = serializedObject.FindProperty("promotable");
        EditorGUILayout.PropertyField(promotableList, new GUIContent("Promotable"), true);

        script.actionList = (ActionList)EditorGUILayout.ObjectField(
            "Action List",
            script.actionList,
            typeof(ActionList),
            false
        );


        GUILayout.Space(10);

        // List of abilities
        for (int i = 0; i < script.abilities.Count; i++)
        {
            Ability ability = script.abilities[i];

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f); // light gray background
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = oldColor;

            ability.name = EditorGUILayout.TextField("Ability Name", ability.name);
            ability.BasicMovement = EditorGUILayout.Toggle("Basic Movement", ability.BasicMovement);
            ability.trigger = (TriggerType)EditorGUILayout.EnumPopup("Trigger", ability.trigger);

            GUILayout.Space(5);
            GUILayout.Label("Actions:", EditorStyles.boldLabel);

            // Darker background for action list
            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f); // light gray background
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = oldColor;

            // Show each action
            for (int j = 0; j < ability.actions.Count; j++)
            {
                GUILayout.BeginHorizontal();

                // Dropdown with available actions (match by name, not reference)
                string[] options = (script.actionList == null) ? new string[1] : System.Array.ConvertAll(script.actionList.Actions, a => a.name);

                int selectedIndex = System.Array.FindIndex(options, n => n == ability.actions[j].name);
                if (selectedIndex < 0) selectedIndex = 0;

                int newIndex = EditorGUILayout.Popup("Action " + j, selectedIndex, options);

                // If changed, update the action’s name
                if (newIndex != selectedIndex)
                {
                    ability.actions[j].name = options[newIndex];
                    ability.actions[j].traits = script.actionList.Actions[newIndex].traits;
                    // optionally: reset grid when type changes
                    ability.actions[j].Init(script.gridSize);
                }

                // Remove action
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    ability.actions.RemoveAt(j);
                    break;
                }

                GUILayout.EndHorizontal();

                // Draw grid for this action
                DrawAction(script, ability.actions[j]);

                GUILayout.Space(20);
            }


            GUILayout.EndVertical(); // end action list box

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Add button "+"
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                if (script.actionList.Actions.Length > 0)
                    ability.actions.Add(script.actionList.Actions[0].Clone());
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // Remove whole ability
            if (GUILayout.Button("Remove Ability"))
            {
                script.abilities.RemoveAt(i);
                break;
            }

            GUILayout.EndVertical();

            GUILayout.Space(30);
        }

        GUILayout.Space(30);

        if (GUILayout.Button("Add Ability"))
        {
            script.abilities.Add(new Ability());
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }


        
 
        serializedObject.ApplyModifiedProperties();
    }

    void DrawAction(ChessPieceData script, Action action){
        // Dropdown for trigger type
        GUILayout.Label(action.name + " Grid", EditorStyles.boldLabel);
        if (action.visualGrid == null) action.Init(script.gridSize);

        if (action.visualGrid == null || action.visualGrid.Length != script.gridSize || action.grid == null || action.grid.Length != script.gridSize * script.gridSize)
        {
            action.Init(script.gridSize);
        }

        DrawVisualGrid(script.gridSize, action.visualGrid, action.grid);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset " + action.name))
            action.Init(script.gridSize);

        GUILayout.EndHorizontal();
    }
 
    private void DrawVisualGrid(int gridSize, Wrapper<Elements>[] visualGrid, int[] intGrid)
    {
        try
        {
            int iterateColorValue = 0;
            GUILayout.BeginVertical();
            for (int i = 0; i < gridSize; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < gridSize; j++)
                {
                    int cellValue = (int)visualGrid[i].values[j];

                    // Set button color
                    Rect rect = GUILayoutUtility.GetRect(30, 30, GUILayout.ExpandWidth(false)); // reserve space

                    // Draw background manually
                    Color oldColor = GUI.color;

                    if (iterateColorValue % 2 == 1)
                    {
                        GUI.color = new Color(0.255f, 0.255f, 0.255f, 1.000f);
                    }
                    else
                    {
                        GUI.color = new Color(0.294f, 0.294f, 0.294f, 1.000f);
                    }

                    if (i == gridSize / 2 && j == gridSize / 2)
                    {
                        GUI.color = Color.red*3 * GUI.color;
                    }
                    else if (cellValue == 1)
                    {
                        GUI.color = Color.green*3 * GUI.color;
                    }
                    else if (cellValue == 2)
                    {
                        GUI.color = new Color(1.000f, 0.769f, 0.000f, 1.000f)*3 * GUI.color;
                    }

                    EditorGUI.DrawRect(rect, GUI.color);
                    GUI.color = oldColor;

                    // Draw button
                    if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                    {
                        if (Event.current.button == 1) // right click to orange
                        {
                            cellValue = Mathf.Clamp(-cellValue+2, 0, 2);
                            Event.current.Use();
                        }
                        else // left click to cycle
                        {
                            cellValue = Mathf.Clamp(-cellValue+1, 0, 1);
                        }
                    }

                    GUI.backgroundColor = oldColor;

                    // Write back to both grid
                    visualGrid[i].values[j] = (Elements)cellValue;
                    intGrid[i * gridSize + j] = cellValue;

                    iterateColorValue++;
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