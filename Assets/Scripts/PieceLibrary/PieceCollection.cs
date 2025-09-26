using UnityEngine;

[System.Serializable]
public class PieceCollection
{
    [SerializeField] public string name;
    [SerializeField] public Sprite[] icons;
    [SerializeField] public PieceDisplayInfo[] pieces;
}
