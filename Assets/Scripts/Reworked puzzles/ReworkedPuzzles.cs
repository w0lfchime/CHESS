using System;
using UnityEngine;

public class ReworkedPuzzles : MonoBehaviour
{
    // string of all piece ids and locations.
    // pieces are stored as sets of two characters in sets of how ever long the board is with / inbetween each row
    public string pieces;

    // string that stors the coms moves and the correct player moves
    // stored in sets of 2 moves, like x1y1x2y2 with slashes inbetween denoting each move
    public string aiMoves;
    public string ourMoves;

    // GAme logic stuff
    public Map map;
    public string reward;
    public string desc;
    public int aiMoveOn = 0;
}
