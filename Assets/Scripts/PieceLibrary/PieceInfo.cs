using UnityEngine;

[System.Serializable]
public class PieceInfo
{
    [SerializeField] public Sprite blackIcon;
    [SerializeField] public Sprite whiteIcon;
    [SerializeField] public string name;
    [SerializeField] public string tagLine;
    [Tooltip("Use HTML tags for Rich Text descriptions!")]
    [SerializeField] public string description;
    [SerializeField] public string abilityDescription;
    [SerializeField] public ChessPiece pieceObject;
}
