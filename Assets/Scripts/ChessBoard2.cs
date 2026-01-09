using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using TMPro;
using System.Collections;
using PurrNet;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Audio;
using Unity.Jobs;

public class ChessBoard2 : NetworkIdentity
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

	public MapData map;
	public GameData gamedata;


	[Header("Options")]
	public bool ScaleChildToTileSize = true;

	[Header("Spawns")]
	public GameObject explosionEffect;

	// 2D array of tile transforms
	public Tile[,] TileLocations;

	public ChessPiece activeChessPiece;
	private List<(Vector2Int, ActionTrait[])> availableMoves = new List<(Vector2Int, ActionTrait[])>();
	public List<ChessPiece> deadWhitePieces = new List<ChessPiece>();
	public List<ChessPiece> deadBlackPieces = new List<ChessPiece>();

	// Stuff for the win screen
	public GameObject gameEndPanel;
	public GameObject puzzleFailPanel;
	public TextMeshProUGUI winText;

	public int puzzleSequence = 0;

	private int abilityClickLayer;

	public int[] turns = new int[] { 0, 0 };
	public bool isPuzzleDone = false;
	public GameObject abilityToggle;
	private GameObject abilityToggleTemp;
	public List<Tile> allClickedOnTiles;
	public List<ChessPiece> allClickedOnPieces;
	public GameObject promoteUI;
	private GameObject promoteUITemp;
	public GameObject piecePrefab;
	public AudioMixerGroup mixerGroup;
	public ChessPieceData pawn;
	public ChessPieceData balloon;

	void Awake()
	{
		Instance = this;
		transform.position = Vector3.zero;

		//WhitePieceMat = GameData.Instance.matList[int.Parse(whiteTeam[whiteTeam.Length - 1])]
		//WhiteTileMat = GameData.Instance.matList[int.Parse(GameData.Instance.teams[GameData.Instance.whiteTeamName][GameData.Instance.teams[GameData.Instance.whiteTeamName].Length - 1])];
		//BlackTileMat = GameData.Instance.matList[int.Parse(GameData.Instance.teams[GameData.Instance.blackTeamName][GameData.Instance.teams[GameData.Instance.blackTeamName].Length - 1])];
	}

	[ContextMenu("Init Board")]
	public void InitBoard()
	{
		map = GameData.Instance.map;
		BoardTileHeight = map.height;
		BoardTileWidth = map.width;

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

		int tileOn = 0;

		for (int r = 0; r < BoardTileHeight; r++)
		{
			for (int c = 0; c < BoardTileWidth; c++)
			{
				if (map.nullTiles[tileOn] != 2)
				{
					Vector3 localPos = new Vector3(
						x0 + c * TileSize,
						0f + map.tileHeights[tileOn],
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
						tilecomp.isWhite = isWhite;
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

				tileOn++;
			}
		}
	}

	[ServerRpc]
	void SendInteractToServer(int y, int x)
	{
		InteractTrigger(TileLocations[x, y], true);
	}

	[ObserversRpc]
	void SendInteractToClient(int y, int x)
	{
		InteractTrigger(TileLocations[x, y], true);
	}

	//triggered when a tile is clicked, a little weird since the first click will give you green spaces and the second click will be on those spaces
	public void InteractTrigger(Tile tile, bool RPC = false, TriggerConditions conditions = TriggerConditions.None)
	{
		if (promoteUITemp != null) return;
		if (abilityToggleTemp != null) Destroy(abilityToggleTemp);

		if ((NetworkManager.main.isServer || NetworkManager.main.isClient) && (GameManager.Instance.CurrentTurn != Team.White) == NetworkManager.main.isServer && !RPC) return;
		if (NetworkManager.main.isClient && !RPC) SendInteractToServer(tile.TileBoardX, tile.TileBoardY);
		if (NetworkManager.main.isServer && !RPC) SendInteractToClient(tile.TileBoardX, tile.TileBoardY);


		Debug.Log("Ran " + tile);
		allClickedOnTiles.Add(tile);
		ChessPiece selected = (tile.tileOccupants.Count > 0) ? tile.tileOccupants[0] : null;

		if (tile == null)
		{
			RemoveHighlightTiles(activeChessPiece);
			return;
		}

		if (selected != null && activeChessPiece == selected) abilityClickLayer++;


		//makes a list of all triggers
		List<TriggerType> triggers = new List<TriggerType>();
		triggers.Add(TriggerType.TurnAction);
		if (GameManager.Instance.turnCount <= 1) triggers.Add(TriggerType.FirstTurnAction);
		if (selected != null && selected.moves == 0) triggers.Add(TriggerType.FirstActionAction);

		if (tile.rend.gameObject.layer == LayerMask.NameToLayer("Highlight") && activeChessPiece && activeChessPiece.team == GameManager.Instance.CurrentTurn)
		{ // if clicking on highlighted tile
			RemoveHighlightTiles(activeChessPiece);
			foreach (TriggerType trigger in triggers)
			{
				TriggerOnePiece(activeChessPiece, trigger, tile, true, !RPC);
			}
			activeChessPiece = null;
		}
		else // if clicking on piece
		{
			RemoveHighlightTiles(activeChessPiece);
			activeChessPiece = null;
			if (tile.tileOccupants.Count > 0)
			{
				allClickedOnPieces.Add(selected);
				activeChessPiece = selected;

				GetHighlightTiles(activeChessPiece, triggers, abilityClickLayer);
				HighlightTiles(activeChessPiece);

				if (activeChessPiece.team != GameManager.Instance.CurrentTurn) RemoveHighlightTiles(activeChessPiece);
			}
		}

		if (selected == null || activeChessPiece != selected) abilityClickLayer = 0;

	}

	//triggered inbetween turns
	public void TurnSwapTrigger(TriggerConditions conditions = TriggerConditions.None)
	{
		// Randomize Frankenstein movesets for the new turn
		RandomizeFrankensteinMovesets();

		TriggerAllPieces(TriggerType.OnTurnSwap, false, conditions);
	}

	//Updates tiles between turns
	public void TileTrigger(TriggerConditions conditions = TriggerConditions.None)
	{
		foreach (Tile tile in TileLocations)
		{
			if (tile != null)
			{
				tile.UpdateEffects(true);
			}
		}
	}

	//Updates tiles between turns
	public void DeathTrigger(ChessPiece piece)
	{
		TriggerOnePiece(piece, TriggerType.OnDeath);
	}

	public void MoveTrigger(ChessPiece piece, TriggerConditions conditions = TriggerConditions.None)
	{
		TriggerOnePiece(piece, TriggerType.OnMove);
	}

	void SpawnPromoteUI(ChessPiece piece)
	{
		if (piece.GetComponent<ChessPieceObject>().chessPieceData.promotable.Count == 0) return;
		promoteUITemp = Instantiate(promoteUI, piece.currentTile.transform.position + Vector3.up * 3, Quaternion.identity);
		foreach (ChessPieceData data in piece.GetComponent<ChessPieceObject>().chessPieceData.promotable)
		{
			GameObject UIPiece = Instantiate(promoteUITemp.transform.GetChild(0).GetChild(0).gameObject, promoteUITemp.transform.GetChild(0));
			UIPiece.transform.GetChild(0).GetComponent<Image>().sprite = data.image;
			UIPiece.GetComponent<Button>().onClick.AddListener(() => Promote(piece, data));
		}
		Destroy(promoteUITemp.transform.GetChild(0).GetChild(0).gameObject);
	}

	void Promote(ChessPiece piece, ChessPieceData data)
	{
		piece.gameObject.GetComponent<MeshFilter>().mesh = data.model;
		piece.gameObject.GetComponent<ChessPieceObject>().chessPieceData = data;
		Destroy(promoteUITemp);

		GameManager.Instance.EndTurn(); // Switch turn
		if (NetworkManager.main.isServer) EndClientTurn();
		if (NetworkManager.main.isClient) EndServerTurn();
	}

	bool TriggerConditionTester(TriggerConditions conditions, ChessPiece piece)
	{
		if (conditions == TriggerConditions.None) return true;

		if (conditions.HasFlag(TriggerConditions.OnWhiteSquare))
		{
			if (piece.currentTile.isWhite) return true;
		}
		if (conditions.HasFlag(TriggerConditions.OnBlackSquare))
		{
			if (!piece.currentTile.isWhite) return true;
		}

		return false;
	}


	private bool TriggerAllPieces(TriggerType trigger, bool endTurn = false, TriggerConditions conditions = TriggerConditions.None)
	{
		List<ChessPiece> pieces = new List<ChessPiece>();
		foreach (Tile tile in TileLocations)
		{
			if (tile != null)
			{
				foreach (ChessPiece piece in tile.tileOccupants) // assuming one occupant for now
				{
					pieces.Add(piece);
				}
			}

		}

		foreach (ChessPiece piece in pieces)
		{
			if (piece == null) continue;
			RunAbilities(piece, piece.GetTileTags(trigger), piece.currentTile, endTurn);
		}

		return true;
	}

	private bool TriggerOnePiece(ChessPiece piece, TriggerType trigger, Tile selectedTile = null, bool layered = false, bool endTurn = false)
	{
		if (layered)
		{
			RunAbilities(piece, GetAbilityLayer(piece, trigger, abilityClickLayer, false), selectedTile, endTurn);
		}
		else
		{
			RunAbilities(piece, piece.GetTileTags(trigger), selectedTile, endTurn);
		}


		return true;
	}

	void RunAbilities(ChessPiece piece, List<Ability_TG> abilities, Tile tile = null, bool endTurn = false)
	{
		List<Ability_TG> allTriggeredAbilities = abilities;

		int did_anything_happen = 0;

		foreach (Ability_TG ability in allTriggeredAbilities)
		{
			foreach (Action_TG action in ability.actions)
			{
				List<(Vector2Int, ActionTrait[])> allTriggeredTiles = action.grid;
				allTriggeredTiles = FilterTiles(piece, allTriggeredTiles);

				bool result = RunTiles(piece, tile, allTriggeredTiles, action.actionEffectMult);
				if (!result) break;
				did_anything_happen += result ? 1 : 0;
			}
			if (ability.sound != null) PlayClipAt(ability.sound, piece.transform.position, mixerGroup, 1);
		}

		if (did_anything_happen > 0)
		{
			piece.moves++;
			if (endTurn)
			{
				if (piece.team == Team.Black && GameData.Instance.map.height - 1 == piece.currentTile.TileBoardY && piece.GetComponent<ChessPieceObject>().chessPieceData.promotable.Count > 0)
				{
					SpawnPromoteUI(piece);
				}
				else if (piece.team == Team.White && 0 == piece.currentTile.TileBoardY && piece.GetComponent<ChessPieceObject>().chessPieceData.promotable.Count > 0)
				{
					SpawnPromoteUI(piece);
				}
				else
				{
					GameManager.Instance.EndTurn(); // Switch turn
					if (NetworkManager.main.isServer) EndClientTurn();
					if (NetworkManager.main.isClient) EndServerTurn();
				}
			}
		}

		if (abilityToggleTemp != null) Destroy(abilityToggleTemp);
	}

	[ServerRpc(requireOwnership: false)]
	void EndServerTurn()
	{
		GameManager.Instance.EndTurn();
	}
	[ObserversRpc(bufferLast: true)]
	void EndClientTurn()
	{
		GameManager.Instance.EndTurn();
	}

	void GetHighlightTiles(ChessPiece piece, List<TriggerType> triggers, int layer)
	{
		foreach (TriggerType trigger in triggers)
		{
			List<Ability_TG> abilityLayer = GetAbilityLayer(piece, trigger, layer, true);
			foreach (Ability_TG ability in abilityLayer)
			{
				if (ability.actions.Count > 0 && TriggerConditionTester(ability.triggerConditions, piece))
				{
					availableMoves.AddRange(ability.actions[0].grid);
				}
			}
		}
		availableMoves = FilterTiles(piece, availableMoves);
	}

	List<Ability_TG> GetAbilityLayer(ChessPiece piece, TriggerType trigger, int layer, bool visual)
	{
		List<List<Ability_TG>> abilityDict = new List<List<Ability_TG>>();
		abilityDict.Add(new List<Ability_TG>());
		var abilities = piece.GetTileTags(trigger, visual);

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

		float scale = piece.GetComponent<ChessPieceObject>().chessPieceData.model_scale_multiplier;
		GameObject abilityToggleIns = Instantiate(abilityToggle, piece.transform.position + transform.up * 2 * scale, Quaternion.identity);
		for (int i = 1; i < abilityDict.Count; i++)
		{
			Instantiate(abilityToggleIns.transform.GetChild(0).GetChild(0).gameObject, abilityToggleIns.transform.GetChild(0));
		}
		abilityToggleIns.transform.GetChild(0).GetChild(selectedLayer).GetComponent<Toggle>().isOn = true;
		abilityToggleTemp = abilityToggleIns;

		return abilityDict[selectedLayer];
	}

	public void RemoveHighlightTiles(ChessPiece selected)
	{
		foreach (Tile tile in TileLocations)
		{
			if (tile != null)
			{
				if (selected == null)
				{
					tile.UnHighlight(0);
				}
				else tile.UnHighlight(Vector2.Distance(new Vector2(tile.TileBoardX, tile.TileBoardY), new Vector2(selected.currentTile.TileBoardX, selected.currentTile.TileBoardY)));
			}

		}

		availableMoves.Clear();
	}

	private void HighlightTiles(ChessPiece selected)
	{
		for (int i = 0; i < availableMoves.Count; i++)
		{
			if (TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y] != null)
			{
				Vector2 pos = new Vector2(TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y].TileBoardX, TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y].TileBoardY);
				TileLocations[availableMoves[i].Item1.x, availableMoves[i].Item1.y].Highlight(Vector2.Distance(pos, new Vector2(selected.currentTile.TileBoardX, selected.currentTile.TileBoardY)));
			}
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

			//check if the tile is out of bounds
			if (tilePosition.x < 0 || tilePosition.y < 0 ||
				tilePosition.x >= TileLocations.GetLength(0) ||
				tilePosition.y >= TileLocations.GetLength(1)) continue;

			//check if the tile is null
			if (TileLocations[tilePosition.x, tilePosition.y] == null)
			{
				continue;
			}

			bool add = false;

			//do all apply
			ChessPiece boardTile = null;
			if (TileLocations[tilePosition.x, tilePosition.y] != null && TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0) boardTile = TileLocations[tilePosition.x, tilePosition.y].tileOccupants[0];

			if (actionTraits.Contains(ActionTrait.apply_to_empty_space) && boardTile == null) add = true;

			if (actionTraits.Contains(ActionTrait.apply_to_ownteam_space) && (boardTile != null && (boardTile.team == cp.team))) add = true;

			if (actionTraits.Contains(ActionTrait.apply_to_opposingteam_space) && (boardTile != null && (boardTile.team != cp.team))) add = true;

			//do all removes

			if (actionTraits.Contains(ActionTrait.remove_obstructed))
			{
				List<Vector2Int> path = GridLine.GetLine(piecePosition, tilePosition);
				foreach (Vector2Int pos in path)
				{
					if (TileLocations[pos.x, pos.y] != null && isObstructed(cp, TileLocations[pos.x, pos.y].obstructed))
					{
						add = false;
						break;
					}
				}
			}

			if (actionTraits.Contains(ActionTrait.remove_unobstructed))
			{
				List<Vector2Int> path = GridLine.GetLine(piecePosition, tilePosition);
				foreach (Vector2Int pos in path)
				{
					if (TileLocations[pos.x, pos.y] != null && !isObstructed(cp, TileLocations[pos.x, pos.y].obstructed))
					{
						add = false;
						break;
					}
				}
			}

			if (TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0 && TileLocations[tilePosition.x, tilePosition.y].tileOccupants[0].GetComponent<ChessPieceObject>().chessPieceData.name == "CorruptKing")
			{
				Debug.Log("ran");
				ChessPieceObject[] pieces = GameManager.Instance.GetCurrentPieceCount();
				Team team = TileLocations[tilePosition.x, tilePosition.y].tileOccupants[0].team;

				int pieceCount = 0;

				for (int i = 0; i < pieces.Length; i++)
				{
					if (pieces[i].team == team)
					{
						pieceCount++;
					}
				}

				if (pieceCount > 1)
				{
					add = false;
				}
			}

			if (TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0 && TileLocations[tilePosition.x, tilePosition.y].tileOccupants[0].GetComponent<ChessPieceObject>().chessPieceData.name == "FatherTime")
			{
				add = false;
			}

			if
			(
				TileLocations[tilePosition.x, tilePosition.y].obstructed != Tile.conditions.None &&
				actionTraits.Contains(ActionTrait.apply_to_empty_space) &&
				!actionTraits.Contains(ActionTrait.apply_to_ownteam_space) &&
				!actionTraits.Contains(ActionTrait.apply_to_opposingteam_space) &&
				isObstructed(cp, TileLocations[tilePosition.x, tilePosition.y].obstructed)
			) add = false;

			(Vector2Int, ActionTrait[]) newTile = (tilePosition, tile.Item2);
			if (add) returnList.Add(newTile);
		}

		return returnList;
	}

	bool isObstructed(ChessPiece cp, Tile.conditions conditions = Tile.conditions.None)
	{
		if (cp.team == Team.White && conditions.HasFlag(Tile.conditions.White)) return true;
		if (cp.team == Team.Black && conditions.HasFlag(Tile.conditions.Black)) return true;
		return false;
	}

	private bool RunTiles(ChessPiece cp, Tile selectedTile, List<(Vector2Int, ActionTrait[])> allTriggeredTiles, float actionEffectMult = 1)
	{
		RemoveHighlightTiles(cp);

		bool wasTileSelected = false;

		Vector2Int previousPosition = new Vector2Int(cp.currentTile.TileBoardY, cp.currentTile.TileBoardX);

		Vector2Int randomPositionFromSelected = -Vector2Int.one;
		if (allTriggeredTiles.Count >= 1) randomPositionFromSelected = allTriggeredTiles[UnityEngine.Random.Range(0, allTriggeredTiles.Count - 1)].Item1;

		foreach ((Vector2Int, ActionTrait[]) tile in allTriggeredTiles)
		{
			Vector2Int tilePosition = tile.Item1;

			ChessPiece ocp = null;
			if (TileLocations[tilePosition.x, tilePosition.y] != null && TileLocations[tilePosition.x, tilePosition.y].tileOccupants.Count > 0) ocp = TileLocations[tilePosition.x, tilePosition.y].GetAllOccupants().ElementAt(0);

			List<ActionTrait> actionTraits = tile.Item2.ToList();

			//check if selected
			if (actionTraits.Contains(ActionTrait.remove_unselected) && selectedTile != TileLocations[tilePosition.x, tilePosition.y]) continue;
			if (actionTraits.Contains(ActionTrait.remove_unselected_far) && (Mathf.Abs(selectedTile.TileBoardY - tilePosition.x) > 1 || Mathf.Abs(selectedTile.TileBoardX - tilePosition.y) > 1))
			{
				continue;
			}
			if (actionTraits.Contains(ActionTrait.remove_all_but_one_random) && tilePosition != randomPositionFromSelected) continue;
			if (actionTraits.Contains(ActionTrait.remove_unselected_line))
			{
				List<Vector2Int> path = GridLine.GetLine(previousPosition, new Vector2Int(selectedTile.TileBoardY, selectedTile.TileBoardX));
				if (!path.Contains(tilePosition))
				{
					continue;
				}
			}

			// stuff for corrupt king
			bool canKill = true;

			if (actionTraits.Contains(ActionTrait.command_dont_kill))
			{
				Debug.Log("Checking if can kill");
				ChessPieceObject[] list = GameManager.Instance.GetCurrentPieceCount();
				Team team = ocp.team;

				int teamCount = 0;

				for (int i = 0; i < list.Length; i++)
				{
					if (list[i].team == team)
					{
						teamCount++;
					}
				}

				if (teamCount <= 1)
				{
					canKill = false;
				}
			}

			wasTileSelected = true;

			//

			//do all lower level commands

			if (actionTraits.Contains(ActionTrait.command_killpiece) && canKill) // if trait kills a piece
			{
				if (ocp != null)
				{
					if (ocp.isLifeline)
					{
						CheckMate(1);
					}
					ocp.Kill();
				}

			}

			if (actionTraits.Contains(ActionTrait.command_pushback))
			{
				Vector2Int targetIndex = tilePosition;
				Vector2Int cpIndex = previousPosition;

				Vector2Int dir = targetIndex - cpIndex;

				dir.x = Mathf.Clamp(dir.x, -1, 1);
				dir.y = Mathf.Clamp(dir.y, -1, 1);

				Vector2Int newIndex = targetIndex + dir;

				TileLocations[targetIndex.x, targetIndex.y].RemovePiece(ocp);

				// Bounds check
				bool outOfBounds =
					newIndex.x < 0 || newIndex.x >= BoardTileHeight ||
					newIndex.y < 0 || newIndex.y >= BoardTileWidth ||
					TileLocations[newIndex.x, newIndex.y] == null ||
					(TileLocations[newIndex.x, newIndex.y] != null && isObstructed(cp, TileLocations[newIndex.x, newIndex.y].obstructed));

				if (outOfBounds)
				{
					ocp.Kill();
				}
				else
				{
					TileLocations[newIndex.x, newIndex.y].AddPiece(ocp);
				}
			}


			if (actionTraits.Contains(ActionTrait.command_goto))// if trait moves the active piece
			{
				float jump = actionTraits.Contains(ActionTrait.animate_jump) ? 10 : 0;
				cp.currentTile.RemovePiece(cp);
				TileLocations[tilePosition.x, tilePosition.y].AddPiece(cp);
				MoveTrigger(cp);
			}

			if (actionTraits.Contains(ActionTrait.command_bring))// if trait brings the other piece here
			{
				float jump = actionTraits.Contains(ActionTrait.animate_jump) ? 10 : 0;
				ocp.currentTile.RemovePiece(ocp);
				TileLocations[previousPosition.x, previousPosition.y].AddPiece(ocp);
			}

			if (actionTraits.Contains(ActionTrait.command_pull_piece))// if trait pulls the other piece closer
			{
				if (ocp != null)
				{
					Vector2Int targetIndex = tilePosition;
					Vector2Int cpIndex = previousPosition;

					// Calculate direction from target to fisherman
					Vector2Int dir = cpIndex - targetIndex;

					// Normalize to single step
					dir.x = Mathf.Clamp(dir.x, -1, 1);
					dir.y = Mathf.Clamp(dir.y, -1, 1);

					Vector2Int newIndex = targetIndex + dir;

					// Check if pulled onto fisherman's square
					if (newIndex == cpIndex)
					{
						// Remove from current position
						TileLocations[targetIndex.x, targetIndex.y].RemovePiece(ocp);

						// Piece dies when pulled onto fisherman
						if (ocp.isLifeline)
						{
							CheckMate(1);
						}
						ocp.Kill();
					}
					else
					{
						// Bounds check and obstruction check
						bool outOfBounds =
							newIndex.x < 0 || newIndex.x >= BoardTileHeight ||
							newIndex.y < 0 || newIndex.y >= BoardTileWidth ||
							TileLocations[newIndex.x, newIndex.y] == null;

						// Check if destination tile is occupied
						bool isOccupied = !outOfBounds &&
							TileLocations[newIndex.x, newIndex.y].tileOccupants.Count > 0;

						// Check if destination tile is obstructed
						bool isBlocked = !outOfBounds &&
							isObstructed(ocp, TileLocations[newIndex.x, newIndex.y].obstructed);

						if (outOfBounds || isOccupied || isBlocked)
						{
							// Don't pull the piece - leave it where it is
							// Optionally could kill it if out of bounds
							if (outOfBounds)
							{
								TileLocations[targetIndex.x, targetIndex.y].RemovePiece(ocp);
								ocp.Kill();
							}
							// If occupied or blocked, do nothing (piece stays in place)
						}
						else
						{
							// Safe to pull
							TileLocations[targetIndex.x, targetIndex.y].RemovePiece(ocp);
							TileLocations[newIndex.x, newIndex.y].AddPiece(ocp);
						}
					}
				}
			}

			if (actionTraits.Contains(ActionTrait.command_swapcolor))// if trait swaps the tile color
			{
				TileLocations[tilePosition.x, tilePosition.y].SwapColor(!TileLocations[tilePosition.x, tilePosition.y].isWhite);
			}


			if (actionTraits.Contains(ActionTrait.spawn_explosion_effect)) //if trait explodes
			{
				ParticleSystem ps = Instantiate(explosionEffect, TileLocations[tilePosition.x, tilePosition.y].transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
				ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
				Material materialInstance = psRenderer.material;
				materialInstance.color = TileLocations[tilePosition.x, tilePosition.y].rend.material.color;
			}

			if (actionTraits.Contains(ActionTrait.spawn_pawn)) //if trait explodes
			{
				ChessPieceObject child = SpawnPiece(pawn.pieceName, new Vector2Int(tilePosition.y, tilePosition.x), cp.team);
			}

			if (actionTraits.Contains(ActionTrait.spawn_balloon))
			{
				ChessPieceObject child = SpawnPiece(balloon.pieceName, new Vector2Int(tilePosition.y, tilePosition.x), cp.team);
			}

			if (actionTraits.Contains(ActionTrait.spawn_water)) // if trait spawns water
			{
				float distance = Vector2.Distance(new Vector2(cp.currentTile.TileBoardX, cp.currentTile.TileBoardY), new Vector2(TileLocations[tilePosition.x, tilePosition.y].TileBoardX, TileLocations[tilePosition.x, tilePosition.y].TileBoardY));
				TileLocations[tilePosition.x, tilePosition.y].AddEffect("water", (int)actionEffectMult, distance);
			}

			if (actionTraits.Contains(ActionTrait.spawn_poison)) // if trait spawns water
			{
				float distance = Vector2.Distance(new Vector2(cp.currentTile.TileBoardX, cp.currentTile.TileBoardY), new Vector2(TileLocations[tilePosition.x, tilePosition.y].TileBoardX, TileLocations[tilePosition.x, tilePosition.y].TileBoardY));
				TileLocations[tilePosition.x, tilePosition.y].AddEffect("poison", (int)actionEffectMult, distance);
			}

			if (actionTraits.Contains(ActionTrait.spawn_slime)) // if trait spawns water
			{
				float distance = Vector2.Distance(new Vector2(cp.currentTile.TileBoardX, cp.currentTile.TileBoardY), new Vector2(TileLocations[tilePosition.x, tilePosition.y].TileBoardX, TileLocations[tilePosition.x, tilePosition.y].TileBoardY));
				TileLocations[tilePosition.x, tilePosition.y].AddEffect("slime", (int)actionEffectMult, distance);
			}

			if (actionTraits.Contains(ActionTrait.spawn_opposing_obstruct)) // if trait spawns water
			{
				float distance = Vector2.Distance(new Vector2(cp.currentTile.TileBoardX, cp.currentTile.TileBoardY), new Vector2(TileLocations[tilePosition.x, tilePosition.y].TileBoardX, TileLocations[tilePosition.x, tilePosition.y].TileBoardY));
				if (cp.team == Team.Black) TileLocations[tilePosition.x, tilePosition.y].AddEffect("scarewhite", (int)actionEffectMult, distance);
				if (cp.team == Team.White) TileLocations[tilePosition.x, tilePosition.y].AddEffect("scareblack", (int)actionEffectMult, distance);
			}

			if (actionTraits.Contains(ActionTrait.command_removetile)) // if trait removes the tile
			{
				Destroy(TileLocations[tilePosition.x, tilePosition.y].gameObject);
			}

			if (actionTraits.Contains(ActionTrait.command_respawn)) // if trait respawns a piece
			{
				if (ocp.originalTile.tileOccupants.Count == 0)
				{
					SpawnPiece("null", new Vector2Int(ocp.originalTile.TileBoardX, ocp.originalTile.TileBoardY), ocp.team, ocp.gameObject);
				}
			}

			//
		}

		// If unhighlighted tile was selected reset the position and return false
		if (!wasTileSelected)
		{

			return false;
		}


		//TriggerAllPieces(TriggerType.OnTurnSwap);

		return true;
	}

	//Checkmate
	public void CheckMate(int team)
	{
		DisplayVictory(team);
	}

	// Randomize movesets for all Frankenstein pieces on the board
	private void RandomizeFrankensteinMovesets()
	{
		foreach (Tile tile in TileLocations)
		{
			if (tile != null)
			{
				foreach (ChessPiece piece in tile.tileOccupants)
				{
					if (piece == null) continue;
					ChessPieceObject pieceObj = piece.GetComponent<ChessPieceObject>();
					if (pieceObj != null && pieceObj.chessPieceData.isFrankenstein)
					{
						SelectRandomMoveset(pieceObj);
					}
				}
			}
		}
	}

	// Select a random moveset for a Frankenstein piece based on weights
	private void SelectRandomMoveset(ChessPieceObject frankensteinPiece)
	{
		ChessPieceData data = frankensteinPiece.chessPieceData;

		if (data.frankensteinMovesets.Count == 0)
		{
			Debug.LogWarning($"Frankenstein piece {frankensteinPiece.name} has no movesets configured!");
			return;
		}

		// Calculate total weight
		float totalWeight = 0f;
		foreach (WeightedMoveset moveset in data.frankensteinMovesets)
		{
			totalWeight += moveset.weightPercentage;
		}

		// Select random value
		float randomValue = UnityEngine.Random.Range(0f, totalWeight);

		// Find the selected moveset
		float cumulativeWeight = 0f;
		foreach (WeightedMoveset moveset in data.frankensteinMovesets)
		{
			cumulativeWeight += moveset.weightPercentage;
			if (randomValue <= cumulativeWeight)
			{
				frankensteinPiece.activeFrankensteinMoveset = moveset.movesetData;
				Debug.Log($"Frankenstein {frankensteinPiece.name} selected moveset: {moveset.movesetData.pieceName}");
				return;
			}
		}

		// Fallback to first moveset
		frankensteinPiece.activeFrankensteinMoveset = data.frankensteinMovesets[0].movesetData;
	}

	public void DisplayVictory(int winningTeam)
	{
		gameEndPanel.SetActive(true);

		if (winningTeam == 0)
		{
			winText.text = "White wins!";
		}
		else
		{
			winText.text = "Black wins!";
		}
	}

	public void movePuzzlePiece()
	{
		if (!isPuzzleDone)
		{
			Puzzles puzzle = GameData.Instance.puzzle;
			string[] tempList = puzzle.blackTeamMovements[turns[0]].Split(" ");
			Tile tile1 = TileLocations[int.Parse(tempList[1]), int.Parse(tempList[0])];
			Tile tile2 = TileLocations[int.Parse(tempList[3]), int.Parse(tempList[2])];

			Debug.Log(tile2);
			Debug.Log(int.Parse(tempList[0]) + " " + int.Parse(tempList[1]) + " " + int.Parse(tempList[2]) + " " + int.Parse(tempList[3]));

			InteractTrigger(tile1);
			StartCoroutine(delayTime(0.5f, tile2));
		}


		turns[0]++;
	}

	public bool checkWhitePuzzle()
	{
		Puzzles puzzle = GameData.Instance.puzzle;
		string[] tempList = puzzle.whiteTeamMovements[turns[1]].Split(" ");

		// for(int i = 0; i < tempList.Length; i += 5)
		// {
		// 	Tile tile = TileLocations[int.Parse(tempList[i+2]), int.Parse(tempList[i+1])];

		// 	if(turns[1] == 0)
		// 	{
		// 		puzzleSequence = int.Parse(tempList[i+3]);
		// 	}

		// 	if(tile.tileOccupants.Count < 1)
		// 	{
		// 		continue;
		// 	}

		// 	if(tile.tileOccupants[0].ID == tempList[i] && tile.tileOccupants[0].team == Team.White && puzzleSequence == int.Parse(tempList[i+3]))
		// 	{
		// 		puzzleSequence = int.Parse(tempList[i+4]);
		// 		return true;
		// 	}
		// }
		for (int i = 0; i < tempList.Length; i++)
		{
			Debug.Log(tempList[i]);
		}
		for (int i = 0; i < tempList.Length; i += 4)
		{
			if (tempList[i] == null || tempList[i] == "")
			{
				continue;
			}
			Tile firstTile = allClickedOnTiles[allClickedOnTiles.Count - 2];
			Tile secondTile = allClickedOnTiles[allClickedOnTiles.Count - 1];

			if (firstTile.TileBoardX == int.Parse(tempList[i]) && firstTile.TileBoardY == int.Parse(tempList[i + 1]) && secondTile.TileBoardX == int.Parse(tempList[i + 2]) && secondTile.TileBoardY == int.Parse(tempList[i + 3]))
			{
				return true;
			}
		}

		return false;
	}

	public void SpawnAllPieces(string[] theirTeam)
	{
		string[] whiteTeam = new string[0];
		string[] blackTeam = new string[0];

		if (NetworkManager.main.isServer)
		{
			whiteTeam = GameData.Instance.teams[GameData.Instance.yourTeamName];
			blackTeam = theirTeam;
		}
		else if (NetworkManager.main.isClient)
		{
			whiteTeam = theirTeam;
			blackTeam = GameData.Instance.teams[GameData.Instance.yourTeamName];
		}
		else
		{
			whiteTeam = GameData.Instance.teams[GameData.Instance.whiteTeamName];
			blackTeam = GameData.Instance.teams[GameData.Instance.blackTeamName];
		}

		int whiteIndex = int.Parse(whiteTeam[whiteTeam.Length - 1]);
		int blackIndex = int.Parse(blackTeam[blackTeam.Length - 1]);

		if (whiteIndex == blackIndex)
		{
			blackIndex--;

			if (blackIndex < 0)
			{
				blackIndex = GameData.Instance.matList.Count - 1;
			}
			else if (blackIndex >= GameData.Instance.matList.Count)
			{
				blackIndex = 0;
			}
		}

		WhitePieceMat = GameData.Instance.matList[whiteIndex].material;
		BlackPieceMat = GameData.Instance.matList[blackIndex].material;

		// GameManager.Instance.CurrentTurn = Team.Black;

		// spawn white pieces. start on where the top left of the team would be
		int pieceOn = 0;

		for (int i = map.startingWhiteTiles[1]; i < map.startingWhiteTiles[1] + 2; i++)
		{
			for (int j = map.startingWhiteTiles[0]; j < map.startingWhiteTiles[0] + 8; j++)
			{
				if (whiteTeam[pieceOn] != null && whiteTeam[pieceOn] != "")
				{
					//Debug.Log("Spawned piece");
					SpawnPiece(whiteTeam[pieceOn], new Vector2Int(j, i), Team.White);
				}

				pieceOn++;
			}
		}

		pieceOn = 0;

		// spawn white pieces
		for (int i = map.startingBlackTiles[1]; i >= map.startingBlackTiles[1] - 2; i--)
		{
			for (int j = map.startingBlackTiles[0]; j >= map.startingBlackTiles[0] - 7; j--)
			{
				if (blackTeam[pieceOn] != null && blackTeam[pieceOn] != "")
				{
					SpawnPiece(blackTeam[pieceOn], new Vector2Int(j, i), Team.Black);
				}

				pieceOn++;
			}
		}
	}


	[ServerRpc(requireOwnership: false)]
	public void SendTeamToServer(string[] team)
	{
		GameManager.Instance.Initialize();
		Debug.Log("SendTeamToServer RPC received");
		SpawnAllPieces(team);
	}

	[ObserversRpc(bufferLast: true)]
	public void SendTeamToClient(string[] team)
	{
		GameManager.Instance.Initialize();
		Debug.Log("SendTeamToClient RPC received");
		SpawnAllPieces(team);
	}
	public void SpawnAllPuzzlePieces()
	{
		Puzzles puzzle = GameData.Instance.puzzle;

		// spawn black
		foreach (PieceBoardData pieceBoardData in puzzle.teamSpawning)
		{
			if (pieceBoardData.data == null) continue;
			SpawnPiece(pieceBoardData.data.pieceName, pieceBoardData.pos, pieceBoardData.team);
		}
	}


	// For regular spawning
	public ChessPieceObject SpawnPiece(string pieceID, Vector2Int boardLoc, Team team, GameObject obj = null)
	{
		ChessPieceData data = null;

		if (obj)
		{

		}
		else
		{
			data = PieceLibrary.Instance.GetPrefab(pieceID);
		}

		if (data == null) return null;

		// Spawn at the tile�s world position
		Vector3 spawnPos = gameObject.transform.position;
		Quaternion spawnRot = Quaternion.identity;

		GameObject pieceGO = Instantiate(piecePrefab, spawnPos, spawnRot);
		pieceGO.GetComponent<MeshFilter>().mesh = data.model;
		pieceGO.transform.localScale = Vector3.one * data.model_scale_multiplier;
		pieceGO.GetComponent<ChessPieceObject>().chessPieceData = data;

		// If the prefab has a ChessPiece script, register it with the tile
		ChessPiece piece = pieceGO.GetComponent<ChessPiece>();

		Renderer rend = pieceGO.GetComponent<Renderer>();

		piece.team = team;
		if (team == Team.White)
		{
			//rend.sharedMaterial = WhitePieceMat;
			Material[] matList = data.whiteMaterialList.ToArray();
			matList[0] = WhitePieceMat;
			pieceGO.GetComponent<MeshRenderer>().materials = matList;
			piece.gameObject.layer = LayerMask.NameToLayer("BlackOutline");
		}
		else
		{
			pieceGO.transform.Rotate(0, 180, 0);
			//rend.sharedMaterial = BlackPieceMat;
			Material[] matList = data.blackMaterialList.ToArray();
			matList[0] = BlackPieceMat;
			pieceGO.GetComponent<MeshRenderer>().materials = matList;
			piece.gameObject.layer = LayerMask.NameToLayer("WhiteOutline");
		}

		pieceGO.transform.Rotate(-90, 180, 0);

		piece.originalTile = TileLocations[boardLoc.y, boardLoc.x];
		TileLocations[boardLoc.y, boardLoc.x].AddPiece(piece);

		// Initialize Frankenstein moveset if this is a Frankenstein piece
		ChessPieceObject pieceObj = piece.GetComponent<ChessPieceObject>();
		if (pieceObj != null && pieceObj.chessPieceData.isFrankenstein)
		{
			SelectRandomMoveset(pieceObj);
		}

		return pieceObj;
	}

	// For debug spawning
	public void SpawnPiece(string pieceID, Vector2Int boardLoc, GameObject obj = null)
	{
		ChessPieceData data = null;

		if (obj)
		{

		}
		else
		{
			data = PieceLibrary.Instance.GetPrefab(pieceID);
		}
		if (data == null) return;

		// Spawn at the tile�s world position
		Vector3 spawnPos = gameObject.transform.position;
		Quaternion spawnRot = Quaternion.identity;

		Team turn = GameManager.Instance.CurrentTurn;

		//if (turn == Team.Black)
		//{
		//	spawnRot *= Quaternion.Euler(0f, 180f, 0f);
		//}

		GameObject pieceGO = Instantiate(piecePrefab, spawnPos, spawnRot);
		pieceGO.GetComponent<MeshFilter>().mesh = data.model;
		pieceGO.transform.localScale = Vector3.one * data.model_scale_multiplier;
		pieceGO.GetComponent<ChessPieceObject>().chessPieceData = data;

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
			piece.transform.Rotate(0, 180, 0);
			piece.team = Team.Black;
			rend.sharedMaterial = BlackPieceMat;
			piece.gameObject.layer = LayerMask.NameToLayer("WhiteOutline");
		}

		piece.transform.Rotate(-90, 0, 0);

		piece.originalTile = TileLocations[boardLoc.y, boardLoc.x];
		TileLocations[boardLoc.y, boardLoc.x].AddPiece(piece);
	}

	public ChessPieceObject getFirstOccupant(Tile tile)
	{
		if (tile.tileOccupants.Count > 0)
		{
			return tile.tileOccupants[0].GetComponent<ChessPieceObject>();
		}
		return null;
	}

	public IEnumerator delayTime(float time, Tile tile)
	{
		yield return new WaitForSeconds(time);
		InteractTrigger(tile);
	}

	public void failedPuzzle()
	{
		puzzleFailPanel.SetActive(true);
		Time.timeScale = 0f;
	}

	public static AudioSource PlayClipAt(AudioClip clip, Vector3 position, AudioMixerGroup outputGroup = null, float volume = 1.0f, float pitch = 1.0f)
	{
		GameObject tempGO = new GameObject("TempAudio"); // create the temp object
		tempGO.transform.position = position; // set its position
		AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source

		aSource.clip = clip; // define the clip
		aSource.volume = volume; // set the volume
		aSource.pitch = pitch; // set the pitch

		// Assign the output Audio Mixer Group if one is provided
		if (outputGroup != null)
		{
			aSource.outputAudioMixerGroup = outputGroup;
		}

		// Set other desired properties here (e.g., spatial blend, priority, etc.)
		// aSource.spatialBlend = 1.0f; // Set to 3D sound if needed

		aSource.Play(); // start the sound
		GameObject.Destroy(tempGO, clip.length); // destroy object after clip duration

		return aSource; // return the AudioSource reference if needed for further dynamic changes
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
			if (new Vector2Int(x0, y0) != start && new Vector2Int(x0, y0) != end) cells.Add(new Vector2Int(x0, y0));

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
