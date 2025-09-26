using UnityEngine;


public class GameManager : MonoBehaviour
{
	[Header("References")]
	public GameCamera Camera;
	public ChessBoard2 Board;

	[Header("Game State")]
	public Team CurrentTurn { get; private set; } = Team.White;


	public CanvasGroup DebugUI;
	public bool DebugUIVIsible = false;

	private void Start()
	{
		// Example: start with White
		Board.InitBoard();
		StartGame(Team.White);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tilde))
		{
			DebugUIVIsible = !DebugUIVIsible;
			ShowCanvasGroup(DebugUI, DebugUIVIsible);	
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

		Debug.Log($"Turn ended. Now it's {CurrentTurn}'s move.");

		// Update camera to face the active team
		if (Camera != null)
			Camera.SetTurn(CurrentTurn == Team.White ? 1 : 0);

		// Later: trigger board highlighting, legal move generation, timers, etc.
	}

	public void ResetGame()
	{
		// Reset state
		CurrentTurn = Team.White;

		// TODO: clear board, respawn pieces, reset UI, etc.
		Debug.Log("Game reset.");
	}

	public static void ShowCanvasGroup(CanvasGroup group, bool show)
	{
		if (group == null) return;

		group.alpha = show ? 1f : 0f;
		group.interactable = show;
		group.blocksRaycasts = show;
	}
}
