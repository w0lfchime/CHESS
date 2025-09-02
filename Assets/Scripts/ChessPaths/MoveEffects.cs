using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Movement effects are intended to function as effects that pieces use on a board.
/// </summary>
/// <param name="board"> The board of pieces. </param>
/// <param name="piece"></param>
/// <param name="position"></param>
/// <param name="targetPosition"></param>
public delegate void MoveEffect(PieceBoard board, ChessPiece piece, Vector2Int position, Vector2Int targetPosition);

public class MoveEffects
{
    /// <summary>
    /// The standard movement effect. Moves the select piece and captures the piece
    /// on the target position if present.
    /// </summary>
    public static void StandardCapture(PieceBoard board, ChessPiece piece, Vector2Int position, Vector2Int target)
    {
        // Capture Logic
        // Movement
    }
    /// <summary>
    /// Enables the piece to be captured via en passant.
    /// </summary>
    public static void PassingMove(PieceBoard board, ChessPiece piece, Vector2Int position, Vector2Int target)
    {
        piece.AddTag("EnPassantable");
        StandardCapture(board, piece, position, target);
    }
    /// <summary>
    /// Captures the piece below the moving piece
    /// (relative to the current team)
    /// </summary>
    public static void EnPassant(PieceBoard board, ChessPiece piece, Vector2Int position, Vector2Int target)
    {
        StandardCapture(board, piece, position, target);
        int direction = piece.team == Team.White ? -1 : 1;

        ChessPiece captureTarget = board[position.x, position.y + direction];
        // Capture Logic
    }
}