using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerGameManager : MonoBehaviour
{
    public List<puzzle> puzzleList = new List<puzzle>();
    public puzzle currentPuzzle;
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

    public void Selectpuzzle(int puzzleNum)
    {
        currentPuzzle = puzzleList[puzzleNum];
        doingPuzzle = true;

        SceneManager.LoadScene("Puzzle Chessboard");
    }
}
