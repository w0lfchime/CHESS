using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class nameMaterialPair
{
    public string key;
    public Material value;
}
public class Tile : MonoBehaviour
{
	public List<nameMaterialPair> effectDictionary = new List<nameMaterialPair>();

	public int TileBoardX, TileBoardY;
	public Renderer rend;

	public List<ChessPiece> tileOccupants = new List<ChessPiece>();
	public Dictionary<string, int> effects = new Dictionary<string, int>();
	public bool obstructed;

	void Awake() => rend = GetComponentInChildren<Renderer>();

	public IEnumerable<ChessPiece> GetAllOccupants()
	{
		foreach (var o in tileOccupants) yield return o;
	}

	public void AddPiece(ChessPiece piece)
	{
		tileOccupants.Add(piece);

		UpdatePieces();
	}

	public void AddEffect(string name, int duration)
	{
        if (effects.ContainsKey(name))
        {
            if(duration > effects[name]) effects[name] = duration;
        }else effects.Add(name, duration);

		UpdateEffects(false);
	}

	public void RemovePiece(ChessPiece piece)
	{
		if (piece == null) return;
		tileOccupants.Remove(piece);

		UpdatePieces();
	}

	public void UpdatePieces()
	{
		foreach (ChessPiece p in tileOccupants)
		{
			p.UpdatePieceData(this);
		}

	}

	public void UpdateEffects(bool turnPass = true)
	{
		Material[] currentMaterials = rend.materials;
		Material[] newMaterialsArray = new Material[effects.Count + 1];
		newMaterialsArray[0] = currentMaterials[0];

		int count = 0;
		List<string> keysToModify = new List<string>();
		foreach (string name in effects.Keys)
		{
			if(turnPass) keysToModify.Add(name);
		}

		foreach (string key in keysToModify)
		{
			effects[key] -= 1;
			if (effects[key] <= 0) effects.Remove(key);
		}
		
		foreach (string name in effects.Keys) // set materials for each
		{
			newMaterialsArray[1 + count] = effectDictionary.FirstOrDefault(kvp => kvp.key == name).value;
			count++;
		}

		rend.materials = newMaterialsArray;

		obstructed = tileOccupants.Count > 0 || effects.ContainsKey("water");

	}

}
