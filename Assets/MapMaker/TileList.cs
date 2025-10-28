using UnityEngine;

[CreateAssetMenu(fileName = "TileList", menuName = "Scriptable Objects/TileList")]
public class TileList : ScriptableObject
{
    public int[,] tileStates = new int[8,8];
}
