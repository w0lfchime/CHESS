using UnityEngine;

[System.Serializable]
public class PieceDisplayInfo
{
    [SerializeField] public Sprite blackIcon;
    [SerializeField] public Sprite whiteIcon;
    [SerializeField] public string name;
    [SerializeField] public string tagLine;
    [SerializeField] public int materialValue;
    [SerializeField] public string description;
    [SerializeField] public string abilityName;
    [SerializeField] public string abilityDescription;
    [SerializeField] public GameObject pieceObject;
}
