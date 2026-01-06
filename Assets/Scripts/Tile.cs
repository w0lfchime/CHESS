using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class nameMaterialPair
{
    public string key;
    public Material value;
}

public class Tile : MonoBehaviour
{
    public List<nameMaterialPair> effectDictionary = new();

    public int TileBoardX, TileBoardY;
    public Renderer rend;

    public List<ChessPiece> tileOccupants = new();
    public Dictionary<string, int> effects = new();

    [Flags]
    public enum conditions
    {
        None  = 0,
        Black = 1 << 0,
        White = 1 << 1,
        All   = Black | White
    }

    public conditions obstructed;
    public bool isWhite;
    private Coroutine raise;

    void Awake() => rend = GetComponentInChildren<Renderer>();

    public IEnumerable<ChessPiece> GetAllOccupants() => tileOccupants;

    public void SwapColor(bool white)
    {
        rend.sharedMaterial = white
            ? ChessBoard2.Instance.WhiteTileMat
            : ChessBoard2.Instance.BlackTileMat;

        isWhite = white;
    }

    public void AddPiece(ChessPiece piece)
    {
        tileOccupants.Add(piece);
        piece.ScaleLongestAxis();
        UpdatePieces();
        UpdateEffects();
    }

    public void RemovePiece(ChessPiece piece)
    {
        if (piece == null) return;
        tileOccupants.Remove(piece);
        UpdatePieces();
        UpdateEffects();
    }

    public void UpdatePieces()
    {
        foreach (var p in tileOccupants)
            p.UpdatePieceData(this);
    }

    public void AddEffect(string name, int duration, float distance = 0f)
    {
        effects[name] = effects.TryGetValue(name, out var d)
            ? Mathf.Max(d, duration)
            : duration;

        UpdateEffects(false, distance);
        StartCoroutine(EffectRaise(-0.3f, distance, effects.Count));
    }

    public void UpdateEffects(bool turnPass = true, float distance = 0f)
    {
        // ---- decrement safely ----
        if (turnPass)
        {
            var keys = effects.Keys.ToList();
            foreach (var k in keys)
            {
                effects[k]--;
                if (effects[k] <= 0)
                    effects.Remove(k);
            }
        }

        // ---- rebuild materials cleanly ----
        var mats = new List<Material> { rend.materials[0] };

        foreach (var kvp in effects)
        {
            var mat = effectDictionary.FirstOrDefault(e => e.key == kvp.Key)?.value;
            if (mat != null)
                mats.Add(mat);
        }

        rend.materials = mats.ToArray();

        // ---- reset and recalc obstruction flags ----
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
        if (raise != null) StopCoroutine(raise);
        raise = StartCoroutine(HighlightRaise(-0.5f, distance));
    }

    public void UnHighlight(float distance)
    {
        if (raise != null) StopCoroutine(raise);
        raise = StartCoroutine(HighlightRaise(0f, distance));
    }

    IEnumerator EffectRaise(float target, float distance, int matIndex)
    {
        yield return new WaitForSeconds(distance / 10f);

        if (matIndex >= rend.materials.Length)
            yield break;

        var mat = rend.materials[matIndex];
        if (!mat.HasFloat("_EffectSize"))
            yield break;

        float size = mat.GetFloat("_EffectSize");

        while (Mathf.Abs(size - target) > 0.01f)
        {
            size = Mathf.Lerp(size, target, 0.05f);
            mat.SetFloat("_EffectSize", size);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator HighlightRaise(float target, float distance)
    {
        yield return new WaitForSeconds(distance / 10f);

        rend.gameObject.layer = LayerMask.NameToLayer("Highlight");

        var mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        float size = mpb.GetFloat("_Size");

        while (Mathf.Abs(size - target) > 0.01f)
        {
            size = Mathf.Lerp(size, target, 0.05f);
            mpb.SetFloat("_Size", size);
            rend.SetPropertyBlock(mpb);
            yield return new WaitForSeconds(0.01f);
        }

        mpb.SetFloat("_Size", target);
        rend.SetPropertyBlock(mpb);

        if (target >= 0)
            rend.gameObject.layer = LayerMask.NameToLayer("Tile");
    }
}
