using UnityEngine;
using System.Collections.Generic;
using System;

public enum LayoutStatus
{
    Success,
    ExceedsValueCap,
    InvalidInput,
    NoLifelinePiece,
}

// FIXME: Remove this temp piece value map when it's included in each piece class
public static class PieceProperties
{
    public static readonly Dictionary<String, int> PieceValues = new Dictionary<string, int>
    {
        {"StandardBishop", 3},
        {"StandardPawn", 1},
        {"StandardKnight", 3},
        {"StandardRook", 5},
        {"StandardQueen", 9},
        {"StandardKing", 0},
    };

    public static readonly HashSet<String> LifelinePieces = new HashSet<string> { "StandardKing" };
}

[Serializable]
public class LayoutData
{
    public String[,] layout;

    public LayoutData(int x = 8, int y = 2)
    {
        layout = new String[x, y];
    }

    public String this[int i, int j]
    {
        get => layout[i, j];
        set => layout[i, j] = value;
    }

    public int GetLength(int dimension)
    {
        return layout.GetLength(dimension);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}

public class PieceLayout
{
    private int valueSum;
    private int valueCap;
    private bool hasLifelinePiece = false;
    private LayoutData layout;
    private Dictionary<String, int> materialValues;
    private HashSet<String> lifelines;

    public PieceLayout(int cap, Dictionary<String, int> valueDict, HashSet<String> lifelineList, int x = 8, int y = 2)
    {
        valueCap = cap;
        valueSum = 0;
        layout = new LayoutData(x,y);
        materialValues = valueDict;
        lifelines = lifelineList;
    }

    public LayoutStatus AddPiece(String piece, int x, int y)
    {
        // Sanity checks
        if (layout == null || x < 0 || x >= layout.GetLength(0) || y < 0 || y >= layout.GetLength(1))
        {
            return LayoutStatus.InvalidInput;
        }


        int pieceValueInt = materialValues[piece];

        if (valueSum + pieceValueInt > valueCap)
        {
            return LayoutStatus.ExceedsValueCap;
        }

        if (layout[x, y] != null)
        {
            // Remove existing piece before adding new one
            if (lifelines.Contains(layout[x, y]))
            {
                // No longer has a lifeline piece
                hasLifelinePiece = false;
            }
            valueSum -= materialValues[layout[x, y]];
            layout[x, y] = null;
        }

        layout[x, y] = piece;
        valueSum += pieceValueInt;
        if (lifelines.Contains(piece))
        {
            hasLifelinePiece = true;
        }
        return LayoutStatus.Success;
    }

    public LayoutStatus RemovePiece(int x, int y)
    {
        // Removing a piece is just adding a null piece
        return AddPiece(null, x, y);
    }

    public LayoutStatus CheckLayout()
    {
        if (!hasLifelinePiece)
        {
            return LayoutStatus.NoLifelinePiece;
        }
        return LayoutStatus.Success;
    }

    public LayoutData GetLayout()
    {
        return layout;
    }

    // Test function to initilize a standard chess layout
    public static PieceLayout StandardChessLayout(Team team)
    {
        PieceLayout layout = new PieceLayout(39, PieceProperties.PieceValues, PieceProperties.LifelinePieces); // Standard chess pieces sum to 39 points

        layout.AddPiece("StandardRook", 0, 0);
        layout.AddPiece("StandardRook", 7, 0);
        layout.AddPiece("StandardKnight", 1, 0);
        layout.AddPiece("StandardKnight", 6, 0);
        layout.AddPiece("StandardBishop", 2, 0);
        layout.AddPiece("StandardBishop", 5, 0);
        if (team == Team.White)
        {
            layout.AddPiece("StandardQueen", 3, 0);
            layout.AddPiece("StandardKing", 4, 0);
        }
        else
        {
            layout.AddPiece("StandardKing", 3, 0);
            layout.AddPiece("StandardQueen", 4, 0);
        }
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            layout.AddPiece("StandardPawn", i, 1);
        }

        return layout;
    }
}