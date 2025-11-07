using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
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


	public CanvasGroup DebugUI;
	public bool DebugUIVIsible = false;

	// Stuff for the win screen
	public GameObject gameEndPanel;
	public TextMeshProUGUI winText;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		Cursor = this.gameObject.GetComponent<CursorManager>();

		// Example: start with White
		Board.InitBoard();
		Board.SpawnAllPieces();
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
	}

	public void StartGame(Team startingTeam)
	{
		CurrentTurn = startingTeam;

		// Update camera for the starting player
		if (Camera != null)
			Camera.SetTurn(CurrentTurn == Team.White ? 1 : 0);

		Debug.Log($"Game started. {CurrentTurn} moves first.");
	}

	public void EndTurn()
	{
		// Switch turn
		CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;

		turnCount++;

		Debug.Log($"Turn ended. Now it's {CurrentTurn}'s move.");

		// Update camera to face the active team
		if (Camera != null)
			Camera.SetTurn(CurrentTurn == Team.White ? 1 : 0);

		// Later: trigger board highlighting, legal move generation, timers, etc.

		TurnStatusText.text = "Turn: " + CurrentTurn.ToString();

		Board.TileTrigger();
		Board.TurnSwapTrigger();
	}

	public void ResetGame()
	{
		SceneManager.LoadScene("Welcome2Chess");
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
