using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TeamSlotRework : MonoBehaviour, IDropHandler
{
    public Image image;
    public GameData gameData;
    public int slotNum;
    public int team;
    public int mat = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
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

            if (dropped.tag == "Dragable Piece" && !(gameData.matValues[team] - mat + PieceProperties.PieceValues[pieceData.pieceId] > gameData.maxMaterial))
            {
                if (team == 0)
                {
                    // singlePlayerGameManager.whiteTeamIds[slotNum] = pieceData.pieceId;
                    gameData.whiteTeamIds[slotNum] = pieceData.pieceId;
                }
                else
                {
                    // singlePlayerGameManager.blackTeamIds[slotNum] = pieceData.pieceId;
                    gameData.blackTeamIds[slotNum] = pieceData.pieceId;
                }

                gameData.matValues[team] -= mat;
                mat = PieceProperties.PieceValues[pieceData.pieceId];
                gameData.matValues[team] += mat;

                image.sprite = pieceData.sprite;

                gameData.updateMatText(team);
            }
        }
        
    }
}
