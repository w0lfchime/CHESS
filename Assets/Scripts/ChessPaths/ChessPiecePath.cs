using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstract behavior for chess piece movement. Is summed for all chess pieces.
/// </summary>

public abstract class ChessPiecePath
{
    /// <summary>
    /// Gets available move locations from the current position.
    /// </summary>
    /// <param name="board">The array of pieces on the board.</param>
    /// <param name="position">The position of the moving piece.</param>
    public abstract List<Vector2Int> GetAvailableMoves(PieceBoard board, Vector2Int position);
    /// <summary>
    /// Gets the movement effect that this path should use when a piece moves along it.
    /// </summary>
    /// <return>Returns the movement effect of this path.</return>
    public virtual MoveEffect Move()
    {
        return MoveEffects.STANDARD_CAPTURE;
    }
}