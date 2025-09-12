using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ChessPieceObject : ChessPiece
{
    public ChessPieceData chessPieceData;

    void Start(){
        //set model for chess piece
        if(GetComponent<MeshFilter>()){
            GetComponent<MeshFilter>().mesh = chessPieceData.model;
        }
    }

    public override List<(Vector2Int, ActionTrait[])> GetTileTags(ref ChessPiece[,] board, int tileCountX, int tileCountY, TriggerType trigger = TriggerType.TurnAction)
    {
        List<(Vector2Int, ActionTrait[])> r = new List<(Vector2Int, ActionTrait[])>();
        
        Vector2Int currentPos = new Vector2Int(currentX, currentY);

        Vector2Int center = new Vector2Int(chessPieceData.gridSize/2, chessPieceData.gridSize/2);

        List<(Vector2Int, ActionTrait[])> taggedTiles = new List<(Vector2Int, ActionTrait[])>();


        foreach(Ability ability in chessPieceData.abilities) //iterate through each ability (needs to be reworked in the future to only work on selected abilities)
        {
            if(ability.trigger == trigger){
                foreach(Action action in ability.actions) // iterate through every action in that ability (maybe only the first action, this really depends on the effects of the ability)
                {
                    for (int y = 0; y < chessPieceData.gridSize; y++) // go through each y tile
                    {
                        for (int x = 0; x < chessPieceData.gridSize; x++){ // go through each x tile
                            if(action.grid[y * chessPieceData.gridSize + x] == 1) //detect if ui tile is selected
                            {
                                Vector2Int pos = new Vector2Int(x, y) - center;
                                pos = new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos; // flip direction based on team and convert local piece position to the chess board tile position

                                taggedTiles.Add((pos, action.traits));
                            }
                        }
                    }
                }
            }
        }

        return taggedTiles; // can be r for testing
    }




    private bool PathClear(Vector2Int start, Vector2Int end, ref ChessPiece[,] board) // old
    {
        Vector2Int dir = new Vector2Int(
            Math.Sign(end.x - start.x),
            Math.Sign(end.y - start.y)
        );

        int cap = 100;
        Vector2Int check = start + dir;
        while (check != end)
        {
            if ((check.x >= 0 && check.x <= 8-1 && check.y >= 0 && check.y <= 8-1) && board[check.x, check.y] != null)
                return false;
            check += dir;

            cap--;
            if(cap <= 0){
                break;
            }
        }
        return true;
    }

    //These are the trigger conditions. When they are called by the manager, they will call on other actions and effects

    //when a piece dies
    public void OnDeath(){

    }

    //when a piece moves
    public void OnMove(){

    }

    //when a piece takes another piece directly (explosions and other similar things will not count)
    public void OnTake(){

    }

    //when a piece moves during the first turn of the game (black and white get a first turn)
    public void OnFirstTurnMove(){

    }

    //the first move a piece makes, like a pawn's first move
    public void OnFirstMove(){

    }

    //wwhen a piece reaches its team's end of the board
    public void OnPromote(){

    }

    //

    //These are actions that are called on by the trigger conditions. For the future, it would be nice to make effected tiles visible from actions effecting them.

    public void Explode(){

    }



    //

}
