using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandardPawnBehavior : ChessPieceBehavior
{
    public override List<Vector2Int> GetAvailableTiles(PieceGrid board, Vector2Int position, Team team)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == Team.White) ? 1 : -1;
        int nextY = position.y + direction;

        // One in front
        if (nextY >= 0 && nextY < board.Height && board[position.x, nextY] == null)
        {
            r.Add(new Vector2Int(position.x, nextY));
        }

        // Kill move right
        if (position.x < board.Width - 1 && nextY >= 0 && nextY < board.Height)
        {
            if (board[position.x + 1, nextY] != null && board[position.x + 1, nextY].team != team)
            {
                r.Add(new Vector2Int(position.x + 1, nextY));
            }
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
}