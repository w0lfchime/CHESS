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
    public abstract List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, Vector2Int position);
    /// <summary>
    /// Executes the movement of a piece. Should capture or otherwise interact with
    /// a piece in the target position if present.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="piece"></param>
    /// <param name="targetPosition"></param>
    
    /* 
     * Programmers note: For alternative means of interaction, override this method
     * in another abstract class if that means of interaction will be reused,
     * such as ramming pieces.
     */
    public virtual void Move(ref ChessPiece[,] board, ChessPiece piece, Vector2Int targetPosition)
    {
        // Logic needed for base move.
    }
}