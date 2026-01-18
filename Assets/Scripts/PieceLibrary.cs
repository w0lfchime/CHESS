using UnityEngine;
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
    [Tooltip("Paths relative to a Resources folder (no leading slash)")]
    public List<string> folderDirectories = new List<string>();

    public static PieceLibrary Instance { get; private set; }

    public List<PieceEntry> pieces = new List<PieceEntry>();
    private Dictionary<string, ChessPieceData> lookup;

    public Sprite emptySprite;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadAllPieces();
        BuildLookup();
    }

    void LoadAllPieces()
    {
        pieces.Clear();

        foreach (string folder in folderDirectories)
        {
            ChessPieceData[] loaded =
                Resources.LoadAll<ChessPieceData>(folder);

            foreach (var data in loaded)
            {
                if (data == null) continue;

                pieces.Add(new PieceEntry
                {
                    id = data.pieceName,
                    data = data
                });
            }
        }
    }

    void BuildLookup()
    {
        lookup = new Dictionary<string, ChessPieceData>();

        foreach (var entry in pieces)
        {
            if (!lookup.ContainsKey(entry.id))
                lookup.Add(entry.id, entry.data);
        }
    }

    public ChessPieceData GetPrefab(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (lookup.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"Piece id '{id}' not found!");
        return null;
    }

    public List<string> GetAllIds()
    {
        return new List<string>(lookup.Keys);
    }

    public List<ChessPieceData> GetAllData()
    {
        return new List<ChessPieceData>(lookup.Values);
    }

    public Sprite NameToImage(string name)
    {
        if (string.IsNullOrEmpty(name))
            return emptySprite;

        if (lookup.TryGetValue(name, out var data))
            return data.image;

        return null;
    }
}
