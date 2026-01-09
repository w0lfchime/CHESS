using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPieceSpawner : MonoBehaviour
{
    public static UIPieceSpawner Instance;
    public GameObject UIPiece;
    public Color lifeLineColor;
    public Sprite emptySprite;
    void Awake()
    {
        Instance = this;
        foreach(PieceEntry entry in PieceLibrary.Instance.pieces)
        {
            if(!entry.data.unlocked)
            {
                if(!PlayerPrefs.HasKey("unlocked " + entry.data.pieceName)) continue;
            }
            GameObject UIPieceIns = Instantiate(UIPiece, transform);
            UIPieceIns.GetComponent<DraggableUIPiece>().image.sprite = entry.data.image;
            UIPieceIns.GetComponent<DraggableUIPiece>().pieceId = entry.data;
            
            if (PieceProperties.LifelinePieces.Contains(entry.id))
            {
                UIPieceIns.GetComponent<Image>().color = lifeLineColor;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void Search(TMP_InputField name)
    {
        List<string> namelist = new List<string>();
        foreach(Transform child in transform)
        {
            if (name.text == "")
            {
                child.gameObject.SetActive(true);
            }
            if (child.GetComponent<DraggableUIPiece>().pieceId.pieceName.ToLower().Contains(name.text.ToLower()))
            {
                child.gameObject.SetActive(true);
            }else child.gameObject.SetActive(false);
        }
    }
}
