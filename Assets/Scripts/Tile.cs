using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public int TileBoardX, TileBoardY;
	private Renderer rend;

	private HashSet<ChessPiece> tileOccupants = new HashSet<ChessPiece>();

	void Awake() => rend = GetComponentInChildren<Renderer>();

	public IEnumerable<ChessPiece> GetAllOccupants()
	{
		foreach (var o in tileOccupants) yield return o;
	}

	public void AddPiece(ChessPiece piece)
	{
		tileOccupants.Add(piece);

		UpdatePieces();
	}

	public void RemovePiece(ChessPiece piece)
	{
		if (piece == null) return;
		tileOccupants.Remove(piece);

		UpdatePieces();
	}

	public void UpdatePieces()
	{
		foreach (ChessPiece p in tileOccupants)
		{
			p.UpdatePieceData(this);
		}
	}

}
