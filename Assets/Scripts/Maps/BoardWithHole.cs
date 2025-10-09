using UnityEngine;

public class BoardWithHole : Map
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pieceLimit = 16;
        length = 8;
        width = 8;
        hasNulls = true;
        whiteStartingTiles = new int[16, 2] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 } };
        blackStartingTiles = new int[16, 2] { { 0, 7 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }, { 0, 6 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 } };
        nullTiles = new int[4, 2] {{3,4}, {4,4}, {3,3}, {4,3}};
    }
}
