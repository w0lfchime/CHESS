using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;
using PurrNet;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;



public class GameManager : NetworkIdentity
{
	public static GameManager Instance { get; private set; }

	[Header("References")]
	public GameCamera Camera;
	public ChessBoard2 Board;
	public CursorManager Cursor;


	[Header("UI References")]
	public TextMeshProUGUI TurnStatusText;
	public InGameOptions pauseMenuManager;

	[Header("Game State")]
	public Team CurrentTurn { get; private set; } = Team.White;
	public int turnCount = 0;
	public bool GamePaused { get; private set; } = false;
	public bool ranPuzzleMoveOnce = false;


	public CanvasGroup DebugUI;
	public bool DebugUIVIsible = false;

	// Stuff for the win screen
	public GameObject gameEndPanel;
	public GameObject puzzleFailPanel;
	public TextMeshProUGUI winText;

	// Stuff for adding a move to the move log
	public GameObject moveTrackerContent;
	public GameObject moveTrackerElementPrefab;
	public bool canMakeTracker = true;

	protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        Camera.SetTurn(asServer ? Team.White : Team.Black);

        if (asServer)
        {
            Debug.Log("SERVER: sending to client");
            Board.SendTeamToClient(GameData.Instance.teams[GameData.Instance.yourTeamName]);

			int seed = Random.Range(0, 999999);
			Random.InitState(seed);
			Board.SetSeed(seed);
        }
        else
        {
            Debug.Log("CLIENT: sending to server");
            Board.SendTeamToServer(GameData.Instance.teams[GameData.Instance.yourTeamName]);
        }
    }

    void Awake()
    {
        Instance = this;
    }

	void Start()
	{
		if(!NetworkManager.main.isServer && !NetworkManager.main.isClient) Initialize();
	}

    public void Initialize()
	{
		Instance = this;

		Cursor = this.gameObject.GetComponent<CursorManager>();

		// Example: start with White
		Board.InitBoard();

		if(!GameData.Instance.isDoingPuzzle)
		{

			if(!NetworkManager.main.isServer && !NetworkManager.main.isClient)
			{
				Board.SpawnAllPieces(null);
			}
		} else
		{
			Board.SpawnAllPuzzlePieces();
		}
		
		StartGame(Team.White);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tilde))
		{
			DebugUIVIsible = !DebugUIVIsible;
			ShowCanvasGroup(DebugUI, DebugUIVIsible);	
		}

		// Handle escape key for pause menu
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("Escape key pressed!");
			TogglePauseMenu();
		}

		if(GameData.Instance.isDoingPuzzle)
		{
			if(CurrentTurn == Team.Black && !ranPuzzleMoveOnce)
			{
				ranPuzzleMoveOnce = true;
				Board.movePuzzlePiece();
			}
		}
	}

	public void StartGame(Team startingTeam)
	{
		CurrentTurn = startingTeam;

		// Update camera for the starting player
		if (Camera != null && (!NetworkManager.main.isServer && !NetworkManager.main.isClient))
			Camera.SetTurn(CurrentTurn);

		Debug.Log($"Game started. {CurrentTurn} moves first.");
	}

	public void EndTurn()
	{		
		if(GameData.Instance.isDoingPuzzle && CurrentTurn == Team.White && !Board.checkWhitePuzzle())
		{
			Debug.Log("Wrong Move");
			StartCoroutine(WaitForIncorrectPuzzle());
			
		} else
		{
			if(CurrentTurn == Team.White && GameData.Instance.isDoingPuzzle)
			{
				Board.turns[1]++;

				if(Board.turns[1] == GameData.Instance.puzzle.whiteTeamMovements.Count)
				{
					Board.DisplayVictory(0);
					Board.winText.text = "Puzzle complete!";
					Board.isPuzzleDone = true;
				}
			}

			// Switch turn
			CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;
			Cursor.currentTurn = CurrentTurn;
			ranPuzzleMoveOnce = false;

			turnCount++;
		
			Debug.Log($"Turn ended. Now it's {CurrentTurn}'s move.");

			// Update camera to face the active team
			if (Camera != null && !GameData.Instance.isDoingPuzzle && (!NetworkManager.main.isServer && !NetworkManager.main.isClient))
				Camera.SetTurn(CurrentTurn);

			// Later: trigger board highlighting, legal move generation, timers, etc.

			TurnStatusText.text = "Turn: " + CurrentTurn.ToString();

			if(canMakeTracker)
			{
				GameObject newMoveSpot = Instantiate(moveTrackerElementPrefab);
				newMoveSpot.GetComponentInChildren<TextMeshProUGUI>().text = Board.allClickedOnPieces[Board.allClickedOnPieces.Count - 1].GetComponent<ChessPieceObject>().chessPieceData.name + " at (" + Board.allClickedOnTiles[Board.allClickedOnTiles.Count - 2].TileBoardX + ", " + Board.allClickedOnTiles[Board.allClickedOnTiles.Count - 2].TileBoardY + ") -> (" + Board.allClickedOnTiles[Board.allClickedOnTiles.Count - 1].TileBoardX + ", " + Board.allClickedOnTiles[Board.allClickedOnTiles.Count - 1].TileBoardY + ")";
				newMoveSpot.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Board.allClickedOnPieces[Board.allClickedOnPieces.Count - 1].GetComponent<ChessPieceObject>().chessPieceData.image;
				newMoveSpot.transform.SetParent(moveTrackerContent.transform);
				newMoveSpot.transform.SetAsFirstSibling();
			}
			
			canMakeTracker = true;

			Board.TileTrigger();
			Board.TurnSwapTrigger();
		}
		
	}

	public void EndTurn(int literallyUseless)
	{
		canMakeTracker = false;
		EndTurn();
	}


	public IEnumerator WaitForIncorrectPuzzle()
	{
		yield return new WaitForSeconds(0.5f);
		Board.failedPuzzle();
	}

	public void ResetGame()
	{
		Time.timeScale = 1f;
		if(!NetworkManager.main.isServer && !NetworkManager.main.isClient){
			SceneManager.LoadScene("Welcome2Chess");
		}
		else
		{
			NetworkManager.main.sceneModule.LoadSceneAsync("Welcome2Chess");
		}
	}

	public void TogglePauseMenu()
	{
		Debug.Log("TogglePauseMenu called");
		
		if (pauseMenuManager != null)
		{
			GamePaused = !GamePaused;
			Debug.Log($"Game paused state: {GamePaused}");
			pauseMenuManager.TogglePauseMenu(GamePaused);
			
			// Pause/unpause the game time
			Time.timeScale = GamePaused ? 0f : 1f;
		}
		else
		{
			Debug.LogError("pauseMenuManager is null! Please assign it in the GameManager inspector.");
		}
	}

	public void ResumeGame()
	{
		GamePaused = false;
		if (pauseMenuManager != null)
		{
			pauseMenuManager.TogglePauseMenu(false);
		}
		Time.timeScale = 1f;
	}

	public void ExitGame()
    {
		SceneManager.LoadScene("MainMenu");
    }

	public static void ShowCanvasGroup(CanvasGroup group, bool show)
	{
		if (group == null) return;

		group.alpha = show ? 1f : 0f;
		group.interactable = show;
		group.blocksRaycasts = show;
	}
}
