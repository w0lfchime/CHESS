using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class unlockableMaterial
{
	public Material material;
	public bool locked = true;
}
public class GameData : MonoBehaviour
{
	public static GameData Instance { get; private set; }

	[Header("Teams")]
	public Dictionary<string, string[]> teams = new Dictionary<string, string[]>();
	public List<unlockableMaterial> matList;
	public List<string[]> teamList = new List<string[]>();
	public List<string> teamNames = new List<string>();

	[Header("Indices")]
	public string whiteTeamName = "";
	public string blackTeamName = "";
	public string yourTeamName = "";
	public bool isDoingPuzzle = false;
	public MapData map;
	public Puzzles puzzle;

	private void Awake()
	{
		// Singleton pattern
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject); // prevent duplicates
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		// Optional: Initialize data here if needed
	}
}
