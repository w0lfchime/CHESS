using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TeamSlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public SinglePlayerGameManager singlePlayerGameManager;
    public int slotNum;
    public int team;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        singlePlayerGameManager = GameObject.Find("Test Object To Carry Data").GetComponent<SinglePlayerGameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
        GameObject dropped = eventData.pointerDrag;
        DraggableUIPiece pieceData = dropped.GetComponent<DraggableUIPiece>();

        if (team == 0)
        {
            singlePlayerGameManager.whiteTeamIds[slotNum] = pieceData.pieceId;
            // add stuff for adding location
        }
        else
        {
            singlePlayerGameManager.blackTeamIds[slotNum] = pieceData.pieceId;
        }

        image.sprite = pieceData.sprite;
    }
}
