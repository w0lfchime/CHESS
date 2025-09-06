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

        Vector2Int center = new Vector2Int(chessPieceData.size/2, chessPieceData.size/2);

        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        List<Vector2Int> possibleBlockedMoves = new List<Vector2Int>();
        List<Vector2Int> possibleTakes = new List<Vector2Int>();
        List<Vector2Int> possibleBlockedTakes = new List<Vector2Int>();

        int[] gridMoves = chessPieceData.abilities.FirstOrDefault(data => data.abilityName == "moves").grid;
        int[] gridTakes = chessPieceData.abilities.FirstOrDefault(data => data.abilityName == "takes").grid;

        for (int y = 0; y < chessPieceData.size; y++){
            for (int x = 0; x < chessPieceData.size; x++){
                if(gridMoves[y * chessPieceData.size + x] == 1){
                    Vector2Int pos = new Vector2Int(x, y) - center;
                    possibleMoves.Add(new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos);
                }
                if(gridMoves[y * chessPieceData.size + x] == 2){
                    Vector2Int pos = new Vector2Int(x, y) - center;
                    possibleBlockedMoves.Add(new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos);
                }
                if(gridTakes[y * chessPieceData.size + x] == 1){
                    Vector2Int pos = new Vector2Int(x, y) - center;
                    possibleTakes.Add(new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos);
                }
                if(gridTakes[y * chessPieceData.size + x] == 2){
                    Vector2Int pos = new Vector2Int(x, y) - center;
                    possibleBlockedTakes.Add(new Vector2Int((team == 0 ? pos.x : -pos.x), (team == 0 ? -pos.y : pos.y))+ currentPos);
                }
            }
        }

        foreach (Vector2Int pos in possibleBlockedMoves){
            if (pos.x >= 0 && pos.x <= tileCountX-1 && pos.y >= 0 && pos.y <= tileCountY-1){
                if (board[pos.x, pos.y] == null && PathClear(currentPos, pos, ref board)){
                    r.Add(pos);
                }
            }
        }
        foreach (Vector2Int pos in possibleBlockedTakes){
            if (pos.x >= 0 && pos.x <= tileCountX-1 && pos.y >= 0 && pos.y <= tileCountY-1){
                if (board[pos.x, pos.y] != null && (chessPieceData.abilities.FirstOrDefault(data => data.abilityName == "friendlyFire").enabled ? true : board[pos.x, pos.y].team != team)){
                    r.Add(pos);
                }
            }
        }
        foreach(Vector2Int pos in possibleMoves){
            if(pos.x >= 0 && pos.x <= tileCountX-1 && pos.y >= 0 && pos.y <= tileCountY-1){
                if(board[pos.x, pos.y] == null){
                    r.Add(pos);
                }
            }
        }
        foreach(Vector2Int pos in possibleTakes){
            if(pos.x >= 0 && pos.x <= tileCountX-1 && pos.y >= 0 && pos.y <= tileCountY-1){
                if(board[pos.x, pos.y] != null && (chessPieceData.abilities.FirstOrDefault(data => data.abilityName == "friendlyFire").enabled ? true : board[pos.x, pos.y].team != team)){
                    r.Add(pos);
                }
            }
        }

        return r;
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

}
