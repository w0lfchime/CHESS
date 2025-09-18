using UnityEngine;
using System.Collections.Generic;



public enum ChessPieceID
{
    None = 0,
    StandardPawn = 1,
    StandardRook = 2,
    StandardKnight = 3,
    StandardBishop = 4,
    StandardQueen = 5,
    StandardKing = 6,
    NecroQueen = 7,
}
public abstract class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public Vector2Int startingPosition;
    public ChessPieceID ID;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    private float jumpAmount;

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == 0) ? Vector3.zero : new Vector3(0f, 180f, 0f));
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition + new Vector3(0, jumpAmount, 0), Time.deltaTime * 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10f);
        jumpAmount -= jumpAmount * .01f;
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //r.Add(new Vector2Int(3, 3));
        //r.Add(new Vector2Int(4, 4));
        //r.Add(new Vector2Int(3, 4));
        //r.Add(new Vector2Int(4, 3));

        return r;
    }

    public virtual List<Ability_TG> GetTileTags(ref ChessPiece[,] board, TriggerType trigger = TriggerType.TurnAction, bool visual = false)
    {
        List<Ability_TG>  r = new List<Ability_TG>();

        //r.Add(new Vector2Int(3, 3));
        //r.Add(new Vector2Int(4, 4));
        //r.Add(new Vector2Int(3, 4));
        //r.Add(new Vector2Int(4, 3));

        return r;
    }

    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        // Default implementation returns no special moves
        return SpecialMove.None;
    }

    public virtual void SetPosition(Vector3 position, float jump = 0, bool force = false)
    {
        desiredPosition = position;
        jumpAmount = jump;
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
}
