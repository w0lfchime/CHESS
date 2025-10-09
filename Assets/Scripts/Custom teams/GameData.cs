using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public List<string[]> whiteTeams = new List<string[]>();
    public List<string[]> blackTeams = new List<string[]>();
    public List<string> whiteTeamNames = new List<string>();
    public List<string> blackTeamNames = new List<string>();
    public int whiteTeamIndex = 0;
    public int blackTeamIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
