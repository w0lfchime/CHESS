using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPieceSpawner : MonoBehaviour
{
    public static UIPieceSpawner Instance;
    public GameObject UIPiece;
    public List<Sprite> Sprites = new List<Sprite>();
    public Color lifeLineColor;
    public Sprite emptySprite;
    void Awake()
    {
        Instance = this;
        foreach(Sprite sprite in Sprites)
        {
            GameObject UIPieceIns = Instantiate(UIPiece, transform);
            UIPieceIns.GetComponent<DraggableUIPiece>().image.sprite = sprite;
            if (PieceProperties.LifelinePieces.Contains(sprite.name))
            {
                UIPieceIns.GetComponent<Image>().color = lifeLineColor;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public Sprite NameToImage(string name)
    {
        if(name=="") return emptySprite;
        foreach(Sprite sprite in Sprites)
        {
            if(sprite.name == name) return sprite;
        }
        return null;
    }
}
