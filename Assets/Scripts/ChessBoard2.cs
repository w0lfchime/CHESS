using UnityEngine;

public class ChessBoard2 : MonoBehaviour
{
	public int BoardTileHeight = 8;  // rows (Z+, top to bottom)
	public int BoardTileWidth = 8;

	public float TileSize = 1f;
	public GameObject tileprefab;
	public Material WhiteTileMat;
	public Material BlackTileMat;

	[Header("Options")]
	public bool ScaleChildToTileSize = true;

	// 2D array of tile transforms
	public Tile[,] TileLocations;

	void Start()
	{
		transform.position = Vector3.zero;
	}

	[ContextMenu("Init Board")]
	public void InitBoard()
	{
		if (tileprefab == null || WhiteTileMat == null || BlackTileMat == null)
		{
			Debug.LogError("Assign tileprefab, WhiteTileMat, and BlackTileMat.");
			return;
		}

		// Clear out old board
		foreach (Transform child in transform)
		{
			DestroyImmediate(child.gameObject);
		}

		TileLocations = new Tile[BoardTileHeight, BoardTileWidth];

		float x0 = -((BoardTileWidth - 1) * 0.5f) * TileSize;
		float z0 = ((BoardTileHeight - 1) * 0.5f) * TileSize;

		for (int r = 0; r < BoardTileHeight; r++)
		{
			for (int c = 0; c < BoardTileWidth; c++)
			{
				Vector3 localPos = new Vector3(
					x0 + c * TileSize,
					0f,
					z0 - r * TileSize
				);

				GameObject tile = Instantiate(tileprefab, transform);
				tile.name = $"Tile_{r}_{c}";
				tile.transform.localPosition = localPos;
				tile.transform.localRotation = Quaternion.identity;

				Tile tilecomp = tile.GetComponent<Tile>();
				if (tilecomp != null)
				{
					tilecomp.TileBoardX = c;
					tilecomp.TileBoardY = r;
				}

				var rend = tile.GetComponentInChildren<Renderer>(true);
				if (rend != null)
				{
					bool isWhite = ((r + c) % 2) == 0;
					rend.sharedMaterial = isWhite ? WhiteTileMat : BlackTileMat;

					if (ScaleChildToTileSize)
					{
						Vector3 s = rend.transform.localScale;
						s.x = TileSize;
						s.y = TileSize; // scale correctly in 2D plane
						rend.transform.localScale = s;
					}
				}

				// Save into 2D array
				TileLocations[r, c] = tilecomp;
			}
		}
	}

	void Update()
	{

	}







	public void SpawnPiece(string pieceID, Vector2Int boardLoc)
	{
		GameObject prefab = PieceLibrary.Instance.GetPrefab(pieceID);
		if (prefab == null) return;

		// Spawn at the tile’s world position
		Vector3 spawnPos = gameObject.transform.position;
		Quaternion spawnRot = Quaternion.identity;

		Team turn = GameManager.Instance.CurrentTurn;

		//if (turn == Team.Black)
		//{
		//	spawnRot *= Quaternion.Euler(0f, 180f, 0f);
		//}

		GameObject pieceGO = Instantiate(prefab, spawnPos, spawnRot);

		// If the prefab has a ChessPiece script, register it with the tile
		ChessPiece piece = pieceGO.GetComponent<ChessPiece>();

		Renderer rend = pieceGO.GetComponent<Renderer>();
		if (turn == Team.White)
		{
			piece.team = Team.White;
			rend.sharedMaterial = WhiteTileMat;
		} 
		else
		{
			piece.team = Team.Black;
			rend.sharedMaterial = BlackTileMat;
		}

		TileLocations[boardLoc.y, boardLoc.x].AddPiece(piece);
	}
}
