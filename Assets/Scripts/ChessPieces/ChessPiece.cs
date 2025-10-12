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
    public int moves;
    public GameObject deathEffect;
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
    public bool _isLifeline;
    public bool isLifeline
    {
        get => _isLifeline;
    }

    //Piece definition
    public ChessPieceType pieceType;
    public HashSet<String> PieceTags { get; private set; }
    public List<ChessPieceBehavior> PieceBehaviors { get; private set; }
    public List<PieceRoutine> Routines { get; private set; }

    public bool IsTileOccupant = true;

    public Tile currentTile;


    //animating
    private Vector3 targetPosition;
    protected Vector3 targetScale = new Vector3(1, 1, 1);
    private float boardY;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler((team == Team.Black) ? Vector3.zero : new Vector3(0f, 180f, 0f));
        PieceTags = new();
        PieceBehaviors = new();
        this.boardY = GameManager.Instance.Board.transform.position.y;
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

    public void UpdatePieceData(Tile tile)
    {
        currentTile = tile;

        targetPosition = currentTile.transform.position;
        targetPosition.y = boardY;
    }

    public void Kill()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, .01f);
    }

    private void Update()
    {
        // Lerp from current to target
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            10f * Time.deltaTime // 10 = speed factor
        );
        
        transform.localScale = Vector3.Lerp(
			transform.localScale,
			targetScale,
			10f * Time.deltaTime // 10 = speed factor
		);
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

    public virtual List<Ability_TG> GetTileTags(TriggerType trigger = TriggerType.TurnAction, bool visual = false)
    {
        List<Ability_TG>  r = new List<Ability_TG>();

        //r.Add(new Vector2Int(3, 3));
        //r.Add(new Vector2Int(4, 4));
        //r.Add(new Vector2Int(3, 4));
        //r.Add(new Vector2Int(4, 3));

        return r;
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


    public void Capture(PieceGrid board, Vector2Int position, bool fromCapture = true)
    {
        Destroy(gameObject);
    }

	internal void SetPosition(Vector3 vector3)
	{
		targetPosition = vector3;
	}

	internal void SetScale(Vector3 vector3)
	{
        targetScale = vector3;
	}

	internal void SetPosition(Vector3 vector3, bool force)
	{
		throw new NotImplementedException();
	}
}
