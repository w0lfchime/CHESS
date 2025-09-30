using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceEntry
{
	public string id;               // e.g. "Knight", "Mage", "Cannon"
	public GameObject prefab;
}

public class PieceLibrary : MonoBehaviour
{
	public static PieceLibrary Instance { get; private set; }

	[SerializeField] private List<PieceEntry> pieces = new List<PieceEntry>();
	private Dictionary<string, GameObject> lookup;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		lookup = new Dictionary<string, GameObject>();
		foreach (var entry in pieces)
		{
			if (!lookup.ContainsKey(entry.id))
				lookup.Add(entry.id, entry.prefab);
		}
	}

	public GameObject GetPrefab(string id)
	{
		if (lookup.TryGetValue(id, out var prefab))
			return prefab;
		Debug.LogError($"Piece id {id} not found!");
		return null;
	}

	// Add this method anywhere inside the class
	public List<string> GetAllIds()
	{
		// Safe copy so callers can't mutate our internal state
		return new List<string>(lookup.Keys);
	}
}
