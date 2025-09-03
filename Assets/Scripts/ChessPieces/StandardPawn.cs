using UnityEngine;
using System.Collections.Generic;

public class StandardPawn : ChessPiece
{

    public override void SetupPiece()
    {
        base.SetupPiece();
        AddTag("Soldier");
    }

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == Team.White) ? 1 : -1;
        int nextY = currentY + direction;

        // One in front
        if (nextY >= 0 && nextY < tileCountY && board[currentX, nextY] == null)
        {
            r.Add(new Vector2Int(currentX, nextY));
        }

        // Two in front
        if (nextY >= 0 && nextY < tileCountY && board[currentX, nextY] == null)
        {
            if (team == Team.White && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
            if (team == Team.Black && currentY == tileCountY - 2 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
        }

        //Code in video was a bit goofy so I revamped it a bit

        // Kill move right
        if (currentX < tileCountX - 1 && nextY >= 0 && nextY < tileCountY)
        {
            if (board[currentX + 1, nextY] != null && board[currentX + 1, nextY].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, nextY));
            }
        }
        // Kill move left
        if (currentX > 0 && nextY >= 0 && nextY < tileCountY)
        {
            if (board[currentX - 1, nextY] != null && board[currentX - 1, nextY].team != team)
            {
                r.Add(new Vector2Int(currentX - 1, nextY));
            }
        }

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == 0) ? 1 : -1;
        if ((team == Team.White && currentY == 6) || (team == Team.Black && currentY == 1))
        {
            return SpecialMove.Promotion;
        }

        //En Passant
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if (board[lastMove[1].x, lastMove[1].y].ID == "StandardPawn") //If last piece was pawn
            {
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) //If last piece was +2 either direction
                {
                    if (board[lastMove[1].x, lastMove[1].y].team != team) //If last piece was not on the same team
                    {
                        if (lastMove[1].y == currentY) //If both pawns are on same Y
                        {
                            if (lastMove[1].x == currentX + 1) //If last piece was on the right
                            {
                                availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            if (lastMove[1].x == currentX - 1) //If last piece was on the left
                            {
                                availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }


        return SpecialMove.None;
    }
}
