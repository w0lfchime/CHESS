using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();


        //Cleaner than tutorial code
        // Check all 8 possible moves for the King
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the current position

                int newX = currentX + x;
                int newY = currentY + y;

                if (newX >= 0 && newX < tileCountX && newY >= 0 && newY < tileCountY)
                {
                    if (board[newX, newY] == null || board[newX, newY].team != team)
                    {
                        r.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMove r = SpecialMove.None;

        var kingMove = moveList.Find(x => x[0].x == 4 && x[0].y == ((team == 0) ? 0 : 7));
        var leftRook = moveList.Find(x => x[0].x == 0 && x[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList.Find(x => x[0].x == 7 && x[0].y == ((team == 0) ? 0 : 7));

        if (kingMove == null && currentX == 4)
        {
            //White team
            if (team == 0)
            {
                //Left rook
                if (leftRook == null)
                {
                    if (board[0, 0].type == ChessPieceType.Rook)
                    {
                        if (board[0, 0].team == 0)
                        {
                            if (board[3, 0] == null && board[2, 0] == null && board[1, 0] == null)
                            {
                                availableMoves.Add(new Vector2Int(2, 0));
                                r = SpecialMove.Castling;
                            }
                        }
                    }
                }
                //Right rook
                if (rightRook == null)
                {
                    if (board[7, 0].type == ChessPieceType.Rook)
                    {
                        if (board[7, 0].team == 0)
                        {
                            if (board[5, 0] == null && board[6, 0] == null)
                            {
                                availableMoves.Add(new Vector2Int(6, 0));
                                r = SpecialMove.Castling;
                            }
                        }
                    }
                }
            }
            else
            {
                //Black team
                //Left rook
                if (leftRook == null)
                {
                    if (board[0, 7].type == ChessPieceType.Rook)
                    {
                        if (board[0, 7].team == 1)
                        {
                            if (board[3, 7] == null && board[2, 7] == null && board[1, 7] == null)
                            {
                                availableMoves.Add(new Vector2Int(2, 7));
                                r = SpecialMove.Castling;
                            }
                        }
                    }
                }
                //Right rook
                if (rightRook == null)
                {
                    if (board[7, 7].type == ChessPieceType.Rook)
                    {
                        if (board[7, 7].team == 1)
                        {
                            if (board[5, 7] == null && board[6, 7] == null)
                            {
                                availableMoves.Add(new Vector2Int(6, 7));
                                r = SpecialMove.Castling;
                            }
                        }
                    }
                }
            }
        }

        // King does not have special moves in this implementation
        return r;
    }
}
