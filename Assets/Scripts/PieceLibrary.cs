using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PieceEntry
{
	public string id;               // e.g. "Knight", "Mage", "Cannon"
	public ChessPieceData data;
}

public class PieceLibrary : MonoBehaviour
{
	public List<string> folderDirectories = new List<string>();
	public static PieceLibrary Instance { get; private set; }

	public List<PieceEntry> pieces = new List<PieceEntry>();
	private Dictionary<string, ChessPieceData> lookup;
	public Sprite emptySprite;

	void Awake()
	{
		List<ChessPieceData> dataList = FindChessPieceDataInFolders(folderDirectories.ToArray());
		foreach(ChessPieceData data in dataList)
		{
			var entry = new PieceEntry();
			entry.id = data.pieceName;
			entry.data = data;
			pieces.Add(entry);
		}

		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		lookup = new Dictionary<string, ChessPieceData>();
		foreach (var entry in pieces)
		{
			if (!lookup.ContainsKey(entry.id))
				lookup.Add(entry.id, entry.data);
		}
	}

	public ChessPieceData GetPrefab(string id)
	{
		if(id==null||id=="") return null;
		if (lookup.TryGetValue(id, out var data))
			return data;
		Debug.LogError($"Piece id {id} not found!");
		return null;
	}

	// Add this method anywhere inside the class
	public List<string> GetAllIds()
	{
		// Safe copy so callers can't mutate our internal state
		return new List<string>(lookup.Keys);
	}

	public static List<ChessPieceData> FindChessPieceDataInFolders(string[] folders)
	{
		string filter = "t:ChessPieceData";
		string[] guids = AssetDatabase.FindAssets(filter, folders);

		var assets = new List<ChessPieceData>(guids.Length);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			ChessPieceData asset = AssetDatabase.LoadAssetAtPath<ChessPieceData>(path);
			if (asset != null)
				assets.Add(asset);
		}

		return assets;
	}

	public Sprite NameToImage(string name)
    {
        if(name=="") return emptySprite;
        foreach(PieceEntry entry in PieceLibrary.Instance.pieces)
        {
            if(entry.id == name) return entry.data.image;
        }
        return null;
    }
}
