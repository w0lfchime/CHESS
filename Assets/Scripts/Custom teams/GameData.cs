using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	public static GameData Instance { get; private set; }

	[Header("Teams")]
	public List<string[]> teamList = new List<string[]>();
	public List<string> teamNames = new List<string>();

	[Header("Indices")]
	public int whiteTeamIndex = 0;
	public int blackTeamIndex = 0;

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
