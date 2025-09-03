using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnPassantPath : ChessPiecePath
{
    public override List<Vector2Int> GetAvailableMoves(PieceGrid board, Vector2Int position, Team team)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == Team.White) ? 1 : -1;
        int nextY = position.y + direction;

        // Kill move right
        if (position.x < board.Width - 1 && nextY >= 0 && nextY < board.Height &&
            board[position.x + 1, nextY] != null && board[position.x + 1, nextY].team != team &&
            board[position.x + 1, nextY].HasTag("EnPassantable"))
        {
            r.Add(new Vector2Int(position.x + 1, nextY));
        }
        // Kill move left
        if (position.x > 0 && nextY >= 0 && nextY < board.Height)
        {
            if (board[position.x - 1, nextY] != null && board[position.x - 1, nextY].team != team)
            {
                r.Add(new Vector2Int(position.x - 1, nextY));
            }
        }

        return r;
    }

    public override MoveEffect Move()
    {
        return MoveEffects.EnPassant;
    }
}