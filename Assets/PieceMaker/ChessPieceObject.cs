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

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        
        Vector2Int currentPos = new Vector2Int(currentX, currentY);

        Vector2Int center = new Vector2Int(chessPieceData.gridSize/2, chessPieceData.gridSize/2);

        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        List<Vector2Int> possibleJumps = new List<Vector2Int>();
        List<Vector2Int> possibleMoveTakes = new List<Vector2Int>();
        List<Vector2Int> possibleJumpTakes = new List<Vector2Int>();

        Dictionary<string, int[]> actionGrids = new Dictionary<string, int[]>();

        List<(Vector2Int, ActionTrait[])> taggedTiles = new List<(Vector2Int, ActionTrait[])>();

        
        foreach(Ability ability in chessPieceData.abilities){
            foreach(Action action in ability.actions){
                for (int y = 0; y < chessPieceData.gridSize; y++){
                    for (int x = 0; x < chessPieceData.gridSize; x++){
                        if(action.grid[y * chessPieceData.gridSize + x] == 1){
                            Vector2Int pos = new Vector2Int(x, y) - center;
                            pos = new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos;

                            taggedTiles.Add((pos, action.traits));
                        }
                    }
                }
            }
        }

        return r; // taggedTIles
    }




    private bool PathClear(Vector2Int start, Vector2Int end, ref ChessPiece[,] board)
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
