using UnityEngine;

public class Map : MonoBehaviour
{
    public int pieceLimit;
    public int length;
    public int width;
    public bool hasNulls;
    public int[,] nullTiles;
    // Starts at BOTTOM LEFT 
    public int[,] whiteStartingTiles;
    // Starts at TOP RIGHT
    public int[,] blackStartingTiles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
