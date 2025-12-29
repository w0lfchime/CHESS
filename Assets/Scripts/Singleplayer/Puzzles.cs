using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject
{
    // format: PieceId x y, PieceId x y...
    public List<string> blackTeamSpawning;
    // format: PieceId startX startY endX endY, ...
    public List<string> blackTeamMovements;

    public List<string> whiteTeamIds;
    public List<string> whiteTeamMovements;

    public Map map;
}
