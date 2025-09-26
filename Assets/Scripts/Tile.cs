using UnityEngine;

public class Tile : MonoBehaviour
{
	public int TileBoardX, TileBoardY; // coords if needed
	private Renderer rend;

	void Awake()
	{
		rend = GetComponentInChildren<Renderer>();
	}

	public void SetHighlight(bool value)
	{
		rend.material.color = value ? Color.yellow : Color.white;
	}

	public void OnTileClicked()
	{
		//Debug.Log($"Tile {TileBoardX},{Tile} clicked");
		// call into board logic if needed
	}
}
