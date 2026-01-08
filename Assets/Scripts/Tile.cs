using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

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

	[Flags]
	public enum conditions
	{
		None    = 0,
		Black   = 1 << 0,
		White     = 1 << 1,
		All   = ~0
	}
	public conditions obstructed;
	public bool isWhite;
	private Coroutine raise;
	private Dictionary<string, Coroutine> tempEffectList = new Dictionary<string, Coroutine>();

	void Awake() => rend = GetComponentInChildren<Renderer>();

	public IEnumerable<ChessPiece> GetAllOccupants()
	{
		foreach (var o in tileOccupants) yield return o;
	}

	public void SwapColor(bool white)
	{
		rend.sharedMaterial = white ? ChessBoard2.Instance.WhiteTileMat : ChessBoard2.Instance.BlackTileMat;
		isWhite = white;
	}

	public void AddPiece(ChessPiece piece)
	{
		tileOccupants.Add(piece);
		piece.ScaleLongestAxis();
		UpdatePieces();
		UpdateEffects();

		if (effects.ContainsKey("poison"))
			piece.Kill();

		if (effects.ContainsKey("slime"))
			piece.interactable = false;
	}

	public void AddEffect(string name, int duration, float distance = 0)
	{
		if (effects.ContainsKey(name))
		{
			if (duration+1 > effects[name]) effects[name] = duration+1;
		}
		else
		{
			effects.Add(name, duration+1);
		}

		UpdateEffects(false, distance);

		// Grow once
		if (tempEffectList.ContainsKey(name))
			StopCoroutine(tempEffectList[name]);

		int matIndex = effects.Keys.ToList().IndexOf(name) + 1;
		tempEffectList[name] = StartCoroutine(EffectRaise(-0.3f, distance, matIndex));
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

	public void UpdateEffects(bool turnPass = true, float distance = 0)
{
	Material[] current = rend.materials;
	List<string> expired = new List<string>();

	if (turnPass)
	{
		foreach (var key in effects.Keys.ToList())
		{
			effects[key]--;
			if (effects[key] <= 0)
				expired.Add(key);
		}
	}

	// Shrink expired effects
	foreach (string name in expired)
	{
		int matIndex = effects.Keys.ToList().IndexOf(name) + 1;

		if (tempEffectList.ContainsKey(name))
			StopCoroutine(tempEffectList[name]);

		tempEffectList[name] = StartCoroutine(RemoveEffectAfterShrink(name, matIndex, distance));
	}

	// rebuild materials for active effects only
	Material[] newMats = new Material[effects.Count + 1];
	newMats[0] = current[0];

	int i = 1;
	foreach (string name in effects.Keys)
	{
		newMats[i] =
			(current.Length > i)
			? current[i]
			: effectDictionary.First(k => k.key == name).value;
		i++;
	}

	rend.materials = newMats;

	// obstruction logic unchanged
	obstructed = conditions.None;

	if (tileOccupants.Count > 0 || effects.ContainsKey("water"))
		obstructed |= conditions.All;

	if (effects.ContainsKey("scarewhite"))
		obstructed |= conditions.White;

	if (effects.ContainsKey("scareblack"))
		obstructed |= conditions.Black;
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
		Material matVar = rend.materials[mat];
		yield return new WaitForSeconds(distance / 10f);

		if(matVar.HasFloat("_EffectSize")){
			float size = rend.materials[mat].GetFloat("_EffectSize");

			while (Mathf.Abs(size - set) > .01f)
			{

				matVar.SetFloat("_EffectSize", size + (set - size) * .05f);
				size = matVar.GetFloat("_EffectSize");

				yield return new WaitForSeconds(.01f);
			}
		}
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

	IEnumerator RemoveEffectAfterShrink(string name, int matIndex, float distance)
{
	yield return EffectRaise(0f, distance, matIndex);

	effects.Remove(name);
	tempEffectList.Remove(name);

	UpdateEffects(false, distance);
}


}
