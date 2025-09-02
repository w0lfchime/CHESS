using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Movement effects are intended to function as effects that pieces use on a board.
/// </summary>
/// <param name="board"></param>
/// <param name="piece"></param>
/// <param name="targetPosition"></param>
public delegate void MoveEffect(PieceBoard board, ChessPiece piece, Vector2Int targetPosition);

public class MoveEffects
{
    public static readonly MoveEffect STANDARD_CAPTURE;

    static MoveEffects()
    {
        STANDARD_CAPTURE = (b, p, t) =>
        {

        };
    }
}