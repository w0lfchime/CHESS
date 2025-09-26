using UnityEngine;

public enum CursorMode { Normal, SpawnPiece }

public class CursorManager : MonoBehaviour
{
	public Camera mainCam;
	public CursorMode mode = CursorMode.Normal;
	public GameObject debugPiecePrefab; // assign in inspector

	private ChessBoard2 board;

	void Start()
	{
		board = FindFirstObjectByType<ChessBoard2>();
		if (mainCam == null)
			mainCam = Camera.main;
	}

	void Update()
	{
		HandleHover();
		HandleClick();
	}

	void HandleHover()
	{
		Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			// Assume each tile has a Tile script
			Tile tile = hit.collider.GetComponent<Tile>();
			if (tile != null)
			{
				
				// highlight or mark tile
				tile.SetHighlight(true);
			}
		}
	}

	void HandleClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Tile tile = hit.collider.GetComponent<Tile>();
				if (tile == null) return;

				switch (mode)
				{
					case CursorMode.Normal:
						tile.OnTileClicked();
						break;

					case CursorMode.SpawnPiece:
						SpawnDebugPiece(tile);
						mode = CursorMode.Normal; // reset
						break;
				}
			}
		}
	}

	void SpawnDebugPiece(Tile tile)
	{
		Instantiate(debugPiecePrefab, tile.transform.position, Quaternion.identity);
		Debug.Log($"Spawned piece at {tile.name}");
	}
}
