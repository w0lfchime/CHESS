using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Movement effects are intended to function as effects that pieces use on a board.
/// </summary>
/// <param name="board"></param>
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
    public static readonly MoveEffect STANDARD_CAPTURE;
    /// <summary>
    /// Enables the piece to be captured via en passant.
    /// </summary>
    public static readonly MoveEffect PASSING_MOVE;
    /// <summary>
    /// Captures the piece below the moving piece
    /// (relative to the current team)
    /// </summary>
    public static readonly MoveEffect EN_PASSANT;

    static MoveEffects()
    {
        STANDARD_CAPTURE = (board, piece, position, target) =>
        {
            // Capture Logic
            // Movement
        };

        PASSING_MOVE = (board, piece, position, target) =>
        {
            piece.AddTag("EnPassantable");
            STANDARD_CAPTURE.Invoke(board, piece, position, target);
        };

        EN_PASSANT = (board, piece, position, target) =>
        {
            STANDARD_CAPTURE.Invoke(board, piece, position, target);
            int direction = piece.team == Team.White ? -1 : 1;

            ChessPiece captureTarget = board[position.x, position.y + direction];
            // Capture Logic
        };
    }
}