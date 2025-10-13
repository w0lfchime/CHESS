using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

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
	private Coroutine raise;

	void Awake() => rend = GetComponentInChildren<Renderer>();

	public IEnumerable<ChessPiece> GetAllOccupants()
	{
		foreach (var o in tileOccupants) yield return o;
	}

	public void AddPiece(ChessPiece piece)
	{
		tileOccupants.Add(piece);

		UpdatePieces();
		UpdateEffects();
	}

	public void AddEffect(string name, int duration, float distance = 0)
	{
		if (effects.ContainsKey(name))
		{
			if (duration > effects[name]) effects[name] = duration;
		}
		else 
		{
			effects.Add(name, duration); 
		}

		UpdateEffects(false, distance);

		StartCoroutine(EffectRaise(-.3f, distance, effects.Count));
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
			if (turnPass) keysToModify.Add(name);
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

	public void Highlight(float distance)
	{
		StopAllCoroutines();
		raise = StartCoroutine(HighlightRaise(-.5f, distance));

	}
	
	public void UnHighlight(float distance)
	{
		StopAllCoroutines();
		raise = StartCoroutine(HighlightRaise(.1f, distance));
    }

    IEnumerator HighlightRaise(float set, float distance)
	{
		yield return new WaitForSeconds(distance / 10f);
		rend.gameObject.layer = LayerMask.NameToLayer("Highlight");

		var mpb = new MaterialPropertyBlock();
		rend.GetPropertyBlock(mpb);	

		float size = mpb.GetFloat("_Size");

		while (Mathf.Abs(size-set) > .01f)
		{
			mpb.SetFloat("_Size", size + (set - size) * .05f);
			rend.SetPropertyBlock(mpb);
			size = mpb.GetFloat("_Size");
			yield return 0;
		}

		if(set > 0) rend.gameObject.layer = LayerMask.NameToLayer("Tile");
    }

	public void UpdateEffects(bool turnPass = true, float distance = 0)
	{
		Material[] currentMaterials = rend.materials;
		Material[] newMaterialsArray = new Material[effects.Count + 1];
		newMaterialsArray[0] = currentMaterials[0];

		int count = 1;
		List<string> keysToModify = new List<string>();
		foreach (string name in effects.Keys)
		{
			if (turnPass) keysToModify.Add(name);
		}

		foreach (string name in keysToModify)
		{
			effects[name] -= 1;
			if (effects[name] <= 0)
			{
				effects.Remove(name);

				Material mat = rend.materials[count];
				newMaterialsArray[count] = mat;
				StartCoroutine(EffectRaise(0, distance,count));
				count++;
			}
		}

		foreach (string name in effects.Keys) // set materials for each
		{
			Material mat = (rend.materials.Length > count) ? rend.materials[count] : effectDictionary.FirstOrDefault(kvp => kvp.key == name).value;
			newMaterialsArray[count] = mat;
			count++;
		}

		rend.materials = newMaterialsArray;

		obstructed = tileOccupants.Count > 0 || effects.ContainsKey("water");

	}

	public void Highlight(float distance)
	{
		if(raise!=null) StopCoroutine(raise);
		raise = StartCoroutine(HighlightRaise(-.5f, distance));

	}

	public void UnHighlight(float distance)
	{
		if (raise != null) StopCoroutine(raise);
		raise = StartCoroutine(HighlightRaise(0, distance));
	}
	
	IEnumerator EffectRaise(float set, float distance, int mat)
    {
		yield return new WaitForSeconds(distance / 10f);

		float size = rend.materials[mat].GetFloat("_EffectSize");

		while (Mathf.Abs(size - set) > .01f)
		{

			rend.materials[mat].SetFloat("_EffectSize", size + (set - size) * .05f);
			size = rend.materials[mat].GetFloat("_EffectSize");

			yield return new WaitForSeconds(.01f);
		}

		if (set >= 0) UpdateEffects(false, distance);
    }

	IEnumerator HighlightRaise(float set, float distance)
	{
		yield return new WaitForSeconds(distance / 10f);
		rend.gameObject.layer = LayerMask.NameToLayer("Highlight");

		var mpb = new MaterialPropertyBlock();
		rend.GetPropertyBlock(mpb);


		float size = mpb.GetFloat("_Size");

		while (Mathf.Abs(size - set) > .01f)
		{
			mpb.SetFloat("_Size", size + (set - size) * .05f);
			rend.SetPropertyBlock(mpb);
			size = mpb.GetFloat("_Size");

			yield return new WaitForSeconds(.01f);
		}

		mpb.SetFloat("_Size", set);
		rend.SetPropertyBlock(mpb);

		if(set >= 0) rend.gameObject.layer = LayerMask.NameToLayer("Tile");

    }

}
