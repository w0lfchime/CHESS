using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugSpawnUI : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private Button spawnButton;
	[SerializeField] private TMP_Dropdown dropdown;

	[Header("State (read-only)")]
	[SerializeField] private string lastSelectedPieceId;

	// Fired when the user presses Spawn (current dropdown selection is passed)
	public System.Action<string> OnPieceChosen;

	private readonly List<string> _ids = new List<string>();

	private void Awake()
	{
		if (spawnButton == null) Debug.LogError("[DebugSpawnUI] Spawn Button not assigned.");
		if (dropdown == null) Debug.LogError("[DebugSpawnUI] TMP_Dropdown not assigned.");

		spawnButton?.onClick.AddListener(OnClickSpawn);
	}


	private bool setupComplete = false;

	private void Update()
	{
		if (!setupComplete)
		{
			PopulateFromLibrary();
			setupComplete = true;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			OnClickSpawn();
		}
	}

	private void PopulateFromLibrary()
	{
		_ids.Clear();
		dropdown.ClearOptions();

		if (PieceLibrary.Instance == null)
		{
			Debug.LogError("[DebugSpawnUI] PieceLibrary.Instance is null. Make sure a PieceLibrary exists in the scene.");
			return;
		}

		var ids = PieceLibrary.Instance.GetAllIds();
		if (ids == null || ids.Count == 0) return;

		_ids.AddRange(ids);
		var options = new List<TMP_Dropdown.OptionData>(_ids.Count);
		for (int i = 0; i < _ids.Count; i++)
			options.Add(new TMP_Dropdown.OptionData(_ids[i]));

		dropdown.AddOptions(options);
		dropdown.SetValueWithoutNotify(0);

		if (_ids.Count > 0)
			lastSelectedPieceId = _ids[0];
	}

	private void OnClickSpawn()
	{
		if (_ids.Count == 0)
		{
			Debug.LogWarning("[DebugSpawnUI] No pieces available.");
			return;
		}

		int index = dropdown.value;
		if (index < 0 || index >= _ids.Count) return;

		lastSelectedPieceId = _ids[index];
		OnPieceChosen?.Invoke(lastSelectedPieceId);

		Debug.Log($"[Debug] Spawn button pressed. Selected piece: {lastSelectedPieceId}");


		GameManager.Instance.Cursor.onNextClick = (tileTransform) =>
		{
			// Find board location (Tile should store its board coords)
			Tile tile = tileTransform.GetComponent<Tile>();
			if (tile == null)
			{
				Debug.LogWarning("[DebugSpawnUI] Clicked object has no Tile component.");
				return;
			}

			Vector2Int loc = new Vector2Int(tile.TileBoardX, tile.TileBoardY);

			GameManager.Instance.Board.SpawnPiece(lastSelectedPieceId, loc);
			Debug.Log($"[Debug] Spawned {lastSelectedPieceId} at {loc}");
		};

	}

}
