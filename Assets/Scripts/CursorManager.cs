using UnityEngine;
using System; // for Action

public class CursorManager : MonoBehaviour
{
	[Header("Raycast")]
	public LayerMask tileLayer;
	public float maxRayDistance = 100f;

	[Header("Cursors")]
	[SerializeField] private TileCursor hoverCursor;    // follows the tile under mouse (live)
	[SerializeField] private TileCursor selectorCursor; // moves only on click (selection)

	// One-shot external callback (e.g., spawn-on-next-click)
	public Action<Transform> onNextClick;

	// Selected Tile (set on mouse click)
	public Tile SelectedTile { get; private set; }

	private Transform _currentTile; // hover hit
	private Camera _cam;

	void Awake()
	{
		_cam = Camera.main;
		if (_cam == null) Debug.LogWarning("[CursorManager] No Camera.main found.");

		if (hoverCursor == null) Debug.LogError("[CursorManager] Hover TileCursor reference not set.");
		if (selectorCursor == null) Debug.LogError("[CursorManager] Selector TileCursor reference not set.");
	}

	void Update()
	{
		CheckTileUnderCursor();
		UpdateHoverCursor();
		HandleClickSelection();
	}

	private void CheckTileUnderCursor()
	{
		if (_cam == null) return;

		Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, tileLayer))
		{
			Transform hitTile = hit.transform;

			// If child mesh hit, step up to parent tagged "Tile"
			if (hitTile.parent != null && hitTile.parent.CompareTag("Tile"))
				hitTile = hitTile.parent;

			_currentTile = hitTile;
		}
		else
		{
			_currentTile = null;
		}
	}

	private void UpdateHoverCursor()
	{
		// Hover cursor follows whatever is under the mouse in real time
		if (hoverCursor == null) return;

		if (_currentTile != null)
		{
			hoverCursor.SetTargetFromTile(_currentTile);
		}
		// else: do nothing; hover cursor will coast/settle where it is
	}

	private void HandleClickSelection()
	{
		if (!Input.GetMouseButtonDown(0)) return;

		// Update SelectedTile only on click
		if (_currentTile != null)
		{
			SelectedTile = _currentTile.GetComponent<Tile>();

			// Move the selector cursor to the newly selected tile
			if (selectorCursor != null)
				selectorCursor.SetTargetFromTile(_currentTile);

			// If a one-shot action is queued, invoke it with the selected tile
			if (onNextClick != null)
			{
				onNextClick.Invoke(_currentTile);
				onNextClick = null; // clear slot
			}
		}
		else
		{
			// Clicked empty space: clear selection (optional)
			SelectedTile = null;

			// Still clear one-shot if you want the click to consume it regardless:
			if (onNextClick != null)
			{
				onNextClick.Invoke(null);
				onNextClick = null;
			}
		}
	}
}
