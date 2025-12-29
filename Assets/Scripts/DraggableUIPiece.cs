//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DraggableUIPiece : MonoBehaviour
{
    public Image image;
    public string pieceId;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pieceId = image.sprite.name;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = new Vector3(1, 1, 0);
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
