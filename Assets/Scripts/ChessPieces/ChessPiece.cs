using UnityEngine;
using System.Collections.Generic;
using System;



public enum Team
{
    White = 0,
    Black = 1,
}
public enum ChessPieceType
{
    /// <summary>
    /// Most pieces are this type.
    /// </summary>
    Normal,
    /// <summary>
    /// The piece/pieces required to take to win the game.
    /// One team wins when they checkmate the last lifeline piece of the other team.
    /// </summary>
    Lifeline,
}
public abstract class ChessPiece : MonoBehaviour
{
    public Team team;
    public int currentX;
    public int currentY;
    [SerializeField] private String _id;
    public String ID
    {
        get => _id;
    }
    [SerializeField] private int _value;
    public int value
    {
        get => _value;
    }
    [SerializeField] private bool _isLifeline;
    public bool isLifeline
    {
        get => _isLifeline;
    }
    public ChessPieceType pieceType;
    public HashSet<String> PieceTags { get; private set; }
    public List<ChessPieceBehavior> PieceBehaviors { get; private set; }
    public List<PieceRoutine> EndOfTurnRoutines { get; private set; }

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == 0) ? Vector3.zero : new Vector3(0f, 180f, 0f));
        PieceTags = new();
        PieceBehaviors = new();
        SetupPiece();
    }

    /// <summary>
    /// Use to set piece tags and behaviors.
    /// </summary>
    public virtual void SetupPiece()
    {
        AddTag("InitialPosition");
    }

    public void AddTag(String tag)
    {
        PieceTags.Add(tag);
    }
    public void RemoveTag(String tag)
    {
        PieceTags.Remove(tag);
    }
    public void AddPath(ChessPieceBehavior path)
    {
        PieceBehaviors.Add(path);
    }
    public void ClearPaths()
    {
        PieceBehaviors.Clear();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Tests if the piece has a specific tag.
    /// </summary>
    /// <param name="tag">The string tag being tested.</param>
    /// <returns>Whether the piece has the tag.</returns>
    public bool HasTag(String tag)
    {
        return PieceTags.Contains(tag);
    }

    public virtual List<Vector2Int> GetAvailableMoves(PieceGrid board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //r.Add(new Vector2Int(3, 3));
        //r.Add(new Vector2Int(4, 4));
        //r.Add(new Vector2Int(3, 4));
        //r.Add(new Vector2Int(4, 3));

        return r;
    }

    public virtual SpecialMove GetSpecialMoves(PieceGrid board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        // Default implementation returns no special moves
        return SpecialMove.None;
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }

    public void Capture(PieceGrid board, Vector2Int position, bool fromCapture = true)
    {
        Destroy(gameObject);
    }
}
