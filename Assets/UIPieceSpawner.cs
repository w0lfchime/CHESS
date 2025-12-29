using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPieceSpawner : MonoBehaviour
{
    public static UIPieceSpawner Instance;
    public GameObject UIPiece;
    public List<Sprite> Sprites = new List<Sprite>();
    void Awake()
    {
        Instance = this;
        foreach(Sprite sprite in Sprites)
        {
            GameObject UIPieceIns = Instantiate(UIPiece, transform);
            UIPieceIns.GetComponent<DraggableUIPiece>().image.sprite = sprite;
        }
    }

    public Sprite NameToImage(string name)
    {
        foreach(Sprite sprite in Sprites)
        {
            if(sprite.name == name) return sprite;
        }
        return null;
    }
}
