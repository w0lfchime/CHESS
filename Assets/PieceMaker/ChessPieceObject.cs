using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Action_TG
{ // TG stands for trait grid
    public float actionEffectMult = 1;
    public List<(Vector2Int, ActionTrait[])> grid = new List<(Vector2Int, ActionTrait[])>();
}

public class Ability_TG
{
    public string name;
    public bool BasicMovement;
    public TriggerConditions triggerConditions;
    public List<Action_TG> actions = new List<Action_TG>();
    public AudioClip sound;
}

public class ChessPieceObject : ChessPiece
{
    public ChessPieceData chessPieceData;
    
    // For Frankenstein piece - stores the currently active moveset
    [HideInInspector]
    public ChessPieceData activeFrankensteinMoveset = null;

    void Start()
    {
        //set model for chess piece
        if (GetComponent<MeshFilter>() && chessPieceData.model!=null)
        {
            GetComponent<MeshFilter>().mesh = chessPieceData.model;
        }

        _isLifeline = chessPieceData.lifeLine;
    }

    public override List<Ability_TG> GetTileTags(TriggerType trigger = TriggerType.TurnAction, bool visual = false)
    {        
        // Use Frankenstein moveset if active, otherwise use base data
        ChessPieceData dataToUse = (chessPieceData.isFrankenstein && activeFrankensteinMoveset != null) 
            ? activeFrankensteinMoveset 
            : chessPieceData;

        Vector2Int center = new Vector2Int(dataToUse.gridSize/2, dataToUse.gridSize/2);


        List<Ability_TG> allTaggedAbilities = new List<Ability_TG>(); // basically all abilities


        foreach(Ability ability in dataToUse.abilities) //iterate through each ability (needs to be reworked in the future to only work on selected abilities)
        {
            if(ability.trigger == trigger){

                Ability_TG ability_TG = new Ability_TG(); // basically all actions
                ability_TG.name = ability.name;
                ability_TG.BasicMovement = ability.BasicMovement;
                ability_TG.triggerConditions = ability.triggerConditions;
                ability_TG.sound = ability.sound;

                int actionCount = 0;

                foreach(Action action in ability.actions) // iterate through every action in that ability (maybe only the first action, this really depends on the effects of the ability)
                {
                    if(visual && actionCount > 0) break;
                    actionCount++;

                    if(!ability.BasicMovement || ability_TG.actions.Count == 0 || !visual) ability_TG.actions.Add(new Action_TG());
                    
                    var action_TG = ability_TG.actions[ability_TG.actions.Count-1];

                    action_TG.actionEffectMult = action.actionEffectMult;

                    for (int y = 0; y < dataToUse.gridSize; y++) // go through each y tile
                    {
                        for (int x = 0; x < dataToUse.gridSize; x++){ // go through each x tile
                            if(action.grid[y * dataToUse.gridSize + x] == 1) //detect if ui tile is selected
                            {
                                Vector2Int pos = new Vector2Int(x, y) - center;
                                pos = new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? pos.y : -pos.y)); // flip direction based on team

                                action_TG.grid.Add((pos, action.traits));
                            }
                            if(action.grid[y * dataToUse.gridSize + x] == 2) //detect if ui tile is selected
                            {
                                Vector2Int pos = new Vector2Int(x-center.x, y-center.y);
                                pos = new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? pos.y : -pos.y));
                                for(int i = 1; i < 16; i++){
                                    action_TG.grid.Add((i*pos, action.traits));
                                }
                            }
                        }
                    }
                }
                allTaggedAbilities.Add(ability_TG);
            }
        }

        return allTaggedAbilities; // can be r for testing
    }

}