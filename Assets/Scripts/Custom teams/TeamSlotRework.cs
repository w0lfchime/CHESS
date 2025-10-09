using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
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

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData != null)
        {
            Debug.Log("Dropped");
            GameObject dropped = eventData.pointerDrag;
            DraggableUIPiece pieceData = dropped.GetComponent<DraggableUIPiece>();

            if (dropped.tag == "Dragable Piece" && !(titleScreenButtons.matValue - mat + PieceProperties.PieceValues[pieceData.pieceId] > titleScreenButtons.maxMaterial))
            {
                titleScreenButtons.tempTeam[slotNum] = pieceData.pieceId;

                titleScreenButtons.matValue -= mat;
                mat = PieceProperties.PieceValues[pieceData.pieceId];
                titleScreenButtons.matValue += mat;

                image.sprite = pieceData.sprite;

                titleScreenButtons.updateMatText();
            }
        }

    }
}
