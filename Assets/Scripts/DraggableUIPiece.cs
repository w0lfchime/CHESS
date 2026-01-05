//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DraggableUIPiece : MonoBehaviour
{
    public Image image;
    public ChessPieceData pieceId;
    public TMP_Text materialValue;
    void Start()
    {
        materialValue.text = pieceId.materialValue.ToString();
    }
    public void SelectPiece()
    {
        if(TitleScreenButtons.Instance.selectedPiece!=null) TitleScreenButtons.Instance.selectedPiece.DeselectPiece();
        GetComponent<Outline>().enabled = true;
        TitleScreenButtons.Instance.selectedPiece = this;
    }

    public void DeselectPiece()
    {
        GetComponent<Outline>().enabled = false;
    }
}
