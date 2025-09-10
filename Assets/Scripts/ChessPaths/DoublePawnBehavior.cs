using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoublePawnBehavior : ChessPieceBehavior
{
    public override List<Vector2Int> GetAvailableTiles(PieceGrid board, Vector2Int position, Team team)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == Team.White) ? 1 : -1;
        int nextY = position.y + direction * 2;

        // Two in front
        if (board[position.x, position.y].HasTag("InitialPosition") &&
            nextY >= 0 && nextY < board.Height && board[position.x, nextY] == null &&
            nextY + direction >= 0 && nextY + direction < board.Height && board[position.x, nextY + direction] == null)
        {
            r.Add(new Vector2Int(position.x, nextY + direction));
        }

        return r;
    }

    public override BehaviorEffect Effect()
    {
        return BehaviorEffects.PassingMove;
    }
}