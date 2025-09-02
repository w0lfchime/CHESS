using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents the board of chess pieces. Can be indexed like a 2D array.
/// When enumerated, skips null entries.
/// 
/// <para>
/// PROGRAMMER'S NOTE: Currently backed by a 2D array. This may change in the future
/// to allow for off-board pieces.
/// </para>
/// </summary>
public class PieceBoard : IEnumerable<ChessPiece>
{
    private readonly ChessPiece[,] chessPieces;
    public int Width => chessPieces.GetLength(0);
    public int Height => chessPieces.GetLength(1);

    public PieceBoard(int width, int height)
    {
        chessPieces = new ChessPiece[width, height];
    }

    public ChessPiece this[int i, int j]
    {
        get => chessPieces[i, j];
        set => chessPieces[i, j] = value;
    }

    public ChessPiece this[Vector2Int p]
    {
        get => this[p.x, p.y];
        set => this[p.x, p.y] = value;
    }

    // Throws a fit if I don't include this.
    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < chessPieces.GetLength(0); i++)
        {
            for (int j = 0; j < chessPieces.GetLength(1); j++)
            {
                if (chessPieces[i, j] != null)
                    yield return chessPieces[i, j];
            }
        }
    }

    // I think this is the one that actually gets used.
    IEnumerator<ChessPiece> IEnumerable<ChessPiece>.GetEnumerator()
    {
        for (int i = 0; i < chessPieces.GetLength(0); i++)
        {
            for (int j = 0; j < chessPieces.GetLength(1); j++)
            {
                if (chessPieces[i, j] != null)
                    yield return chessPieces[i, j];
            }
        }
    }
}