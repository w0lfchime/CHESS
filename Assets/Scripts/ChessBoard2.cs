using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChessBoard2 : MonoBehaviour
{
	public static ChessBoard2 Instance { get; private set; }
	public int BoardTileHeight = 8;  // rows (Z+, top to bottom)
	public int BoardTileWidth = 8;

	public float TileSize = 1f;
	public GameObject tileprefab;
	public Material WhiteTileMat;
	public Material BlackTileMat;
	public Material WhitePieceMat;
	public Material BlackPieceMat;

	[Header("Options")]
	public bool ScaleChildToTileSize = true;

	[Header("Spawns")]
	public GameObject explosionEffect;

	// 2D array of tile transforms
	public Tile[,] TileLocations;

	private ChessPiece activeChessPiece;
	private List<(Vector2Int, ActionTrait[])> availableMoves = new List<(Vector2Int, ActionTrait[])>();
	private List<Ability_TG> allTriggeredAbilities = new List<Ability_TG>();

	private List<ChessPiece> deadWhitePieces = new List<ChessPiece>();
	private List<ChessPiece> deadBlackPieces = new List<ChessPiece>();

	private int abilityClickLayer;

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
		if (tile == null)
		{
			RemoveHighlightTiles();
			return;
		}

		ChessPiece selected = (tile.tileOccupants.Count > 0) ? tile.tileOccupants[0] : null;

		if (selected != null && activeChessPiece == selected) abilityClickLayer++;


		//makes a list of all triggers
		List<TriggerType> triggers = new List<TriggerType>();
		triggers.Add(TriggerType.TurnAction);
		if (GameManager.Instance.turnCount <= 1) triggers.Add(TriggerType.FirstTurnAction);
		if (selected != null && selected.moves == 0) triggers.Add(TriggerType.FirstActionAction);

		if (tile.rend.gameObject.layer == LayerMask.NameToLayer("Highlight") && activeChessPiece && activeChessPiece.team == GameManager.Instance.CurrentTurn)
		{ // if clicking on highlighted tile
			foreach (TriggerType trigger in triggers)
			{
				TriggerOnePiece(activeChessPiece, trigger, tile, true);
			}
			RemoveHighlightTiles();
			activeChessPiece = null;
		}
		else // if clicking on piece
		{
			RemoveHighlightTiles();
			activeChessPiece = null;
			if (tile.tileOccupants.Count > 0)
			{
				GetHighlightTiles(selected, triggers, abilityClickLayer);
				HighlightTiles();

				activeChessPiece = selected;
				if (activeChessPiece.team != GameManager.Instance.CurrentTurn) RemoveHighlightTiles();
			}
		}
		
		if (selected == null || activeChessPiece != selected) abilityClickLayer = 0;
		
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
				RunAbilities(piece, piece.GetTileTags(trigger), tile);
			}
		}

		return true;
	}

	private bool TriggerOnePiece(ChessPiece piece, TriggerType trigger, Tile selectedTile = null, bool layered = false)
	{
		if (layered) {
			print(abilityClickLayer);
			RunAbilities(piece, GetAbilityLayer(piece, trigger, abilityClickLayer), selectedTile);
		} else {
			RunAbilities(piece, piece.GetTileTags(trigger), selectedTile);
		}


		return true;
	}

	void RunAbilities(ChessPiece piece, List<Ability_TG> abilities, Tile tile = null)
	{
		allTriggeredAbilities = abilities;

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

		if (did_anything_happen > 0)
		{
			piece.moves++;
			GameManager.Instance.EndTurn(); // Switch turn
		}
	}


	void GetHighlightTiles(ChessPiece piece, List<TriggerType> triggers, int layer)
	{ // plan on making a thing to display the second action in an ability, just to show an effect of what will happen. Will likely have a bool on each action to enable this.
		foreach (TriggerType trigger in triggers)
		{
			List <Ability_TG> abilityLayer = GetAbilityLayer(piece, trigger, layer);
			foreach (Ability_TG ability in abilityLayer)
			{
				if (ability.actions.Count > 0)
				{
					availableMoves.AddRange(ability.actions[0].grid);
				}
			}
		}
		availableMoves = FilterTiles(piece, availableMoves);
	}

	List<Ability_TG> GetAbilityLayer(ChessPiece piece, TriggerType trigger, int layer)
	{
		List<List<Ability_TG>> abilityDict = new List<List<Ability_TG>>();
		abilityDict.Add(new List<Ability_TG>());
		var abilities = piece.GetTileTags(trigger, true);

		if (abilities.Count == 0) return new List<Ability_TG>();

		foreach (Ability_TG ability in abilities)
		{
			if (!ability.BasicMovement)
			{
				abilityDict.Add(new List<Ability_TG>());
			}
			abilityDict[abilityDict.Count - 1].Add(ability);
		}

		int selectedLayer = (layer >= abilityDict.Count) ? layer % abilityDict.Count : layer;

		return abilityDict[selectedLayer];
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
			Vector2Int piecePosition = new Vector2Int(cp.currentTile.TileBoardY, cp.currentTile.TileBoardX);
			Vector2Int tilePosition = new Vector2Int(tile.Item1.y, tile.Item1.x) + piecePosition;
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

			if (actionTraits.Contains(ActionTrait.remove_obstructed))
			{
				List<Vector2Int> path = GridLine.GetLine(piecePosition, tilePosition);
				foreach (Vector2Int pos in path)
				{
					if (TileLocations[pos.x, pos.y].tileOccupants.Count > 0)
					{ 
						add = false;
						print(TileLocations[pos.x, pos.y]);
						break;
					}
				}
			}

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

			if (actionTraits.Contains(ActionTrait.command_killpiece) && (TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0)) // if trait kills a piece
			{
				ChessPiece ocp = TileLocations[tilePosition.x, tilePosition.y].GetAllOccupants().ElementAt(0);

				if (ocp.deathEffect != null) Instantiate(ocp.deathEffect, ocp.transform.position, Quaternion.identity);

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

			if (actionTraits.Contains(ActionTrait.command_goto))// if trait moves the active piece
			{
				float jump = actionTraits.Contains(ActionTrait.animate_jump) ? 10 : 0;
				cp.currentTile.RemovePiece(cp);
				TileLocations[tilePosition.x, tilePosition.y].AddPiece(cp);
			}

			if (actionTraits.Contains(ActionTrait.spawn_explosion_effect))
			{
				Instantiate(explosionEffect, TileLocations[tilePosition.x, tilePosition.y].transform.position, Quaternion.identity);
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

	public void SpawnAllPieces()
	{
		gameData = GameObject.Find("GameData").GetComponent<GameData>();
		string[] whiteTeam = gameData.whiteTeams[gameData.whiteTeamIndex];
		string[] blackTeam = gameData.whiteTeams[gameData.blackTeamIndex];

		int pieceOn = 0;
		// GameManager.Instance.CurrentTurn = Team.Black;

		// spawn black pieces
		for (int i = 7; i > 5; i--)
		{
			for (int j = 7; j >= 0; j--)
			{
				if (blackTeam[pieceOn] != null && blackTeam[pieceOn] != "")
				{
					Debug.Log("Spawned piece");
					SpawnPiece(blackTeam[pieceOn], new Vector2Int(j, i), Team.Black);
				}

				pieceOn++;
			}
		}

		pieceOn = 0;
		
		// spawn white pieces
		for (int i = 1; i >= 0; i--)
		{
			for (int j = 7; j >= 0; j--)
			{
				if (whiteTeam[pieceOn] != null && whiteTeam[pieceOn] != "")
				{
					SpawnPiece(whiteTeam[pieceOn], new Vector2Int(j, i), Team.White);
				}

				pieceOn++;
			}
		}
	}




	// For regular spawning
	public void SpawnPiece(string pieceID, Vector2Int boardLoc, Team team)
	{
		GameObject prefab = PieceLibrary.Instance.GetPrefab(pieceID);
		if (prefab == null) return;

		// Spawn at the tile�s world position
		Vector3 spawnPos = gameObject.transform.position;
		Quaternion spawnRot = Quaternion.identity;

		//if (turn == Team.Black)
		//{
		//	spawnRot *= Quaternion.Euler(0f, 180f, 0f);
		//}

		GameObject pieceGO = Instantiate(prefab, spawnPos, spawnRot);

		// If the prefab has a ChessPiece script, register it with the tile
		ChessPiece piece = pieceGO.GetComponent<ChessPiece>();

		Renderer rend = pieceGO.GetComponent<Renderer>();

		piece.team = team;
		if (team == Team.White)
		{
			rend.sharedMaterial = WhiteTileMat;
		}
		else
		{
			rend.sharedMaterial = BlackTileMat;
		}

		TileLocations[boardLoc.y, boardLoc.x].AddPiece(piece);
	}

	// For debug spawning
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
			rend.sharedMaterial = WhitePieceMat;
			piece.gameObject.layer = LayerMask.NameToLayer("BlackOutline");
		}
		else
		{
			piece.team = Team.Black;
			rend.sharedMaterial = BlackPieceMat;
			piece.gameObject.layer = LayerMask.NameToLayer("WhiteOutline");
		}

		TileLocations[boardLoc.y, boardLoc.x].AddPiece(piece);
	}
}

public static class GridLine
{
    // Returns all grid cells (Vector2Int) that a line from start → end passes through
    public static List<Vector2Int> GetLine(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;
        int err2;

        while (true)
        {
			if(new Vector2Int(x0, y0) != start && new Vector2Int(x0, y0) != end) cells.Add(new Vector2Int(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            err2 = 2 * err;

            if (err2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (err2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return cells;
    }
}
