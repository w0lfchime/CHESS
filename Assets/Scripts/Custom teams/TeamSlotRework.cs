using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TeamSlotRework : MonoBehaviour, IDropHandler
{
    public TitleScreenButtons titleScreenButtons;
    public Image image;
    public GameData gameData;
    public int slotNum;
    public int mat = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        titleScreenButtons = GameObject.Find("TitleScreenButtons").GetComponent<TitleScreenButtons>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData != null)
        {
            Debug.Log("Dropped");
            GameObject dropped = eventData.pointerDrag;
            DraggableUIPiece pieceData = dropped.GetComponent<DraggableUIPiece>();

            if (dropped.tag == "Draggable Piece" && !(titleScreenButtons.matValue - mat + PieceProperties.PieceValues[pieceData.pieceId] > titleScreenButtons.maxMaterial))
            {
                if (titleScreenButtons.lifelineCount() > 0 && (pieceData.pieceId == "StandardKing"))
                {
                    titleScreenButtons.editMatTextStuff("Too many lifelines");
                }
                else
                {
                    titleScreenButtons.tempTeam[slotNum] = pieceData.pieceId;
                    int currentMat = mat * -1;

                    titleScreenButtons.matValue -= mat;
                    mat = PieceProperties.PieceValues[pieceData.pieceId];
                    titleScreenButtons.matValue += mat;
                    currentMat += mat;

                    image.sprite = pieceData.sprite;

                    titleScreenButtons.updateMatText();
                    titleScreenButtons.editMatTextNotError((currentMat >= 0) ? "+" + currentMat : currentMat.ToString());
                }
                
            }
            else if(dropped.tag == "Draggable Piece")
            {
                titleScreenButtons.editMatTextStuff("Max 39");
            }
        }

    }
}
