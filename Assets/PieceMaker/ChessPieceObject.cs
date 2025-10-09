using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Action_TG
{ // TG stands for trait grid
    public List<(Vector2Int, ActionTrait[])> grid = new List<(Vector2Int, ActionTrait[])>();
}

public class Ability_TG
{
    public string name;
    public bool BasicMovement;
    public List<Action_TG> actions = new List<Action_TG>();
}

public class ChessPieceObject : ChessPiece
{
    public ChessPieceData chessPieceData;

    void Start(){
        //set model for chess piece
        if(GetComponent<MeshFilter>()){
            GetComponent<MeshFilter>().mesh = chessPieceData.model;
        }
    }

    public override List<Ability_TG> GetTileTags(TriggerType trigger = TriggerType.TurnAction, bool visual = false)
    {        

        Vector2Int center = new Vector2Int(chessPieceData.gridSize/2, chessPieceData.gridSize/2);


        List<Ability_TG> allTaggedAbilities = new List<Ability_TG>(); // basically all abilities


        foreach(Ability ability in chessPieceData.abilities) //iterate through each ability (needs to be reworked in the future to only work on selected abilities)
        {
            if(ability.trigger == trigger){

                Ability_TG ability_TG = new Ability_TG(); // basically all actions
                ability_TG.name = ability.name;
                ability_TG.BasicMovement = ability.BasicMovement;

                int actionCount = 0;

                foreach(Action action in ability.actions) // iterate through every action in that ability (maybe only the first action, this really depends on the effects of the ability)
                {
                    if(visual && actionCount > 0) break;
                    actionCount++;

                    if(!ability.BasicMovement || ability_TG.actions.Count == 0 || !visual) ability_TG.actions.Add(new Action_TG());
                    
                    var action_TG = ability_TG.actions[ability_TG.actions.Count-1];

                    for (int y = 0; y < chessPieceData.gridSize; y++) // go through each y tile
                    {
                        for (int x = 0; x < chessPieceData.gridSize; x++){ // go through each x tile
                            if(action.grid[y * chessPieceData.gridSize + x] == 1) //detect if ui tile is selected
                            {
                                Vector2Int pos = new Vector2Int(x, y) - center;
                                pos = new Vector2Int((team == 0 ? -pos.x : pos.x), (team == 0 ? -pos.y : pos.y)); // flip direction based on team

                                action_TG.grid.Add((pos, action.traits));
                            }
                            if(action.grid[y * chessPieceData.gridSize + x] == 2) //detect if ui tile is selected
                            {
                                Vector2Int pos = new Vector2Int(x-center.x, y-center.y);
                                pos = new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y));
                                for(int i = 0; i < 16; i++){
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
