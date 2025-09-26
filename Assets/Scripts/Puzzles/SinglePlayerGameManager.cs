using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerGameManager : MonoBehaviour
{
    // Scripts
    public List<puzzle> puzzleList = new List<puzzle>();
    public List<Map> mapList = new List<Map>();
    public Map currentMap;
    public puzzle currentPuzzle;

    // Team data
    public string[] whiteTeamIds;
    public string[] blackTeamIds;
    public int[,] whiteTeamLocations;
    public int[,] blackTeamLocations;

    // Team creation
    public GameObject teamSelectPanel;
    public GameObject mainMenuPanel;
    public GameObject whiteTeamContent;
    public GameObject blackTeamContent;
    public GameObject slotPrefab;

    // misc
    public bool doingPuzzle = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void makeSlot(int team, int slotNum, GameObject parent)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlot teamSlot = slot.GetComponent<TeamSlot>();

        teamSlot.slotNum = slotNum;
        teamSlot.team = team;

        slot.transform.SetParent(parent.transform);
    }

    public void Selectpuzzle(int puzzleNum)
    {
        currentPuzzle = puzzleList[puzzleNum];
        doingPuzzle = true;

        SceneManager.LoadScene("Puzzle Chessboard");
    }

    public void SelectMap(int mapNum)
    {
        currentMap = mapList[mapNum];
        whiteTeamIds = new string[currentMap.pieceLimit];
        blackTeamIds = new string[currentMap.pieceLimit];
        whiteTeamLocations = new int[currentMap.pieceLimit, 2];
        blackTeamLocations = new int[currentMap.pieceLimit, 2];

        for (int i = 0; i < currentMap.pieceLimit; i++)
        {
            makeSlot(1, i, blackTeamContent);
        }

        for (int i = currentMap.pieceLimit - 1; i >= 0; i--)
        {
            makeSlot(0, i, whiteTeamContent);
        }

        teamSelectPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        // SceneManager.LoadScene("Puzzle Chessboard");
    }

    public void OnBack()
    {
        whiteTeamIds = null;
        blackTeamIds = null;
        whiteTeamLocations = null;
        blackTeamLocations = null;
        currentMap = null;

        teamSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnStart()
    {
        SceneManager.LoadScene("Puzzle Chessboard");
    }
}
