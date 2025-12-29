using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject
{
    // format PieceId x y
    public List<string> blackTeamSpawning;
    public List<string> blackTeamMovements;

    public List<string> whiteTeamIds;
    public List<string> whiteTeamMovements;

    public Map map;
}
