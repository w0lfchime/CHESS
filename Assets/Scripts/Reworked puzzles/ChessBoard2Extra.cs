using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChessBoard2Extra : MonoBehaviour
{
	public static ChessBoard2Extra Instance { get; private set; }
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

	// Stuff for puzzles
	public ReworkedPuzzles currentPuzzle;

	private ChessPiece activeChessPiece;
	private List<(Vector2Int, ActionTrait[])> availableMoves = new List<(Vector2Int, ActionTrait[])>();
	private List<Ability_TG> allTriggeredAbilities = new List<Ability_TG>();

	private List<ChessPiece> deadWhitePieces = new List<ChessPiece>();
	private List<ChessPiece> deadBlackPieces = new List<ChessPiece>();

	void Start()
	{
		Instance = this;
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

	//triggered when a tile is clicked, a little weird since the first click will give you green spaces and the second click will be on those spaces
	public void InteractTrigger(Tile tile)
	{
		if (tile.rend.gameObject.layer == LayerMask.NameToLayer("Highlight") && activeChessPiece)
		{
			TriggerOnePiece(activeChessPiece, TriggerType.TurnAction, tile);
			RemoveHighlightTiles();
			activeChessPiece = null;
		}
		else
		{
			RemoveHighlightTiles();
			if (tile.tileOccupants.Count > 0)
			{
				ChessPiece selected = tile.tileOccupants[0];
				GetHighlightTiles(selected, TriggerType.TurnAction);
				HighlightTiles();

				activeChessPiece = selected;
			}
		}
	}

	//triggered inbetween turns
	public void TurnTrigger(Tile tile)
	{

	}


	private bool TriggerAllPieces(TriggerType trigger, Tile[] tiles)
	{
		foreach (Tile tile in tiles)
		{
			foreach (ChessPiece piece in tile.tileOccupants) // assuming one occupant for now
			{
				RunAbilities(piece, trigger, tile);
			}
		}

		return true;
	}

	private bool TriggerOnePiece(ChessPiece piece, TriggerType trigger, Tile selectedTile = null)
	{
		RunAbilities(piece, trigger, selectedTile);

		return true;
	}

	void RunAbilities(ChessPiece piece, TriggerType trigger, Tile tile = null)
	{
		allTriggeredAbilities = piece.GetTileTags(trigger);

		int did_anything_happen = 0;

		foreach (Ability_TG ability in allTriggeredAbilities)
		{
			foreach (Action_TG action in ability.actions)
			{
				List<(Vector2Int, ActionTrait[])> allTriggeredTiles = action.grid;
				allTriggeredTiles = FilterTiles(piece, allTriggeredTiles);
				bool result = RunTiles(piece, tile, allTriggeredTiles);
				if (!result) break;
				did_anything_happen += result ? 1 : 0;
			}
		}

		if (did_anything_happen > 0) GameManager.Instance.EndTurn(); // Switch turn
	}


	void GetHighlightTiles(ChessPiece piece, TriggerType trigger)
	{ // plan on making a thing to display the second action in an ability, just to show an effect of what will happen. Will likely have a bool on each action to enable this.
		allTriggeredAbilities = piece.GetTileTags(trigger, true);
		availableMoves.Clear();
		foreach (Ability_TG ability in allTriggeredAbilities)
		{
			if (ability.actions.Count > 0)
			{
				availableMoves.AddRange(ability.actions[0].grid);
			}
		}
		availableMoves = FilterTiles(piece, availableMoves);
	}

	private void RemoveHighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++)
		{
			TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y].rend.gameObject.layer = LayerMask.NameToLayer("Tile");
		}

		availableMoves.Clear();
	}

	private void HighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++)
		{
			TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y].rend.gameObject.layer = LayerMask.NameToLayer("Highlight");
		}
	}

	//checks each tile in available moves and iterates through the tags. 
	private List<(Vector2Int, ActionTrait[])> FilterTiles(ChessPiece cp, List<(Vector2Int, ActionTrait[])> grid)
	{
		List<(Vector2Int, ActionTrait[])> returnList = new List<(Vector2Int, ActionTrait[])>();
		foreach ((Vector2Int, ActionTrait[]) tile in grid)
		{
			Vector2Int tilePosition = new Vector2Int(tile.Item1.y, tile.Item1.x) + new Vector2Int(cp.currentTile.TileBoardY, cp.currentTile.TileBoardX);
			List<ActionTrait> actionTraits = tile.Item2.ToList();

			try
			{
				var test = TileLocations[tilePosition.x, tilePosition.y];
			}
			catch
			{
				continue;
			}

			bool add = false;

			//do all apply
			ChessPiece boardTile = null;
			if (TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0) boardTile = TileLocations[tilePosition.x, tilePosition.y].tileOccupants[0];

			if (actionTraits.Contains(ActionTrait.apply_to_empty_space) && boardTile == null) add = true;

			if (actionTraits.Contains(ActionTrait.apply_to_ownteam_space) && (boardTile != null && (boardTile.team == cp.team))) add = true;

			if (actionTraits.Contains(ActionTrait.apply_to_opposingteam_space) && (boardTile != null && (boardTile.team != cp.team))) add = true;

			//do all removes

			(Vector2Int, ActionTrait[]) newTile = (tilePosition, tile.Item2);
			if (add) returnList.Add(newTile);
		}

		return returnList;
	}

	private bool RunTiles(ChessPiece cp, Tile selectedTile, List<(Vector2Int, ActionTrait[])> allTriggeredTiles)
	{
		bool wasTileSelected = false;

		Vector2Int previousPosition = new Vector2Int(cp.currentTile.TileBoardX, cp.currentTile.TileBoardY);

		foreach ((Vector2Int, ActionTrait[]) tile in allTriggeredTiles)
		{
			Vector2Int tilePosition = tile.Item1;

			List<ActionTrait> actionTraits = tile.Item2.ToList();

			//check if selected

			if (actionTraits.Contains(ActionTrait.remove_unselected) && selectedTile != TileLocations[tilePosition.x, tilePosition.y]) continue;

			wasTileSelected = true;

			//

			//do all lower level commands

			if (actionTraits.Contains(ActionTrait.command_killpiece)) // if trait kills a piece
			{
				ChessPiece ocp = TileLocations[tilePosition.x, tilePosition.y].GetAllOccupants().ElementAt(0);

				if (ocp.deathEffect != null) Instantiate(ocp.deathEffect, new Vector3(tilePosition.x - 3.5f, 1f, tilePosition.y - 3.5f), Quaternion.identity);

				if (ocp != null)
				{
					if (ocp.isLifeline)
					{
						CheckMate(1);
					}
					Vector3 bounds = new Vector3(BoardTileWidth * TileSize, BoardTileHeight * TileSize, 0);
					float deathSpacing = .3f;
					float deathSize = 0.3f;
					float yOffset = 0.3f;
					float deathHeight = 0f;
					var deadPieces = (ocp.team == Team.White) ? deadWhitePieces : deadBlackPieces;
					deadPieces.Add(ocp);
					ocp.SetScale(Vector3.one * deathSize);
					ocp.SetPosition(
						new Vector3(((ocp.team == Team.White) ? 8 : -1) * TileSize, yOffset + deathHeight, ((ocp.team == Team.White) ? -1 : 8) * TileSize) - bounds
						+ new Vector3((TileSize / 2), 0, (TileSize / 2))
						+ (Vector3.forward * deathSpacing * ((ocp.team == Team.White) ? 1 : -1)) * deadPieces.Count
					);

					TileLocations[tilePosition.x, tilePosition.y].RemovePiece(ocp);
				}

			}

			if (actionTraits.Contains(ActionTrait.command_goto))
			{  // if trait moves the active piece
				float jump = actionTraits.Contains(ActionTrait.animate_jump) ? 10 : 0;
				cp.currentTile.RemovePiece(cp);
				TileLocations[tilePosition.x, tilePosition.y].AddPiece(cp);
			}

			//
		}

		// If unhighlighted tile was selected reset the position and return false
		if (!wasTileSelected)
		{

			return false;
		}

		RemoveHighlightTiles();

		//TriggerAllPieces(TriggerType.OnTurnSwap);

		return true;
	}

	//Checkmate
	private void CheckMate(int team)
	{
		DisplayVictory(team);
	}
	private void DisplayVictory(int winningTeam)
	{
		//victoryScreen.SetActive(true);
		//victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
	}







	public void SpawnPiece(string pieceID, Vector2Int boardLoc)
	{
		GameObject prefab = PieceLibrary.Instance.GetPrefab(pieceID);
		if (prefab == null) return;

		// Spawn at the tile�s world position
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

	public void SpawnAllPieces()
	{
		string[] rows = currentPuzzle.pieces.Split('/');

		for (int i = 0; i < rows.Length; i++)
		{
			string[] currentRow = rows[i].Split(' ');

			for (int j = 0; j < 8; j++)
			{
				string currentPiece = PiecesHashmap.PieceIds[currentRow[j]];

				if (!(currentPiece == "None"))
				{
					SpawnPiece(currentPiece, new Vector2Int(i, j));
				}

			}
		}
	}
}
