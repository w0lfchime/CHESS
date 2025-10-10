using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TeamSlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public GameData gameData;
    public int slotNum;
    public int team;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
        GameObject dropped = eventData.pointerDrag;
        DraggableUIPiece pieceData = dropped.GetComponent<DraggableUIPiece>();


        image.sprite = pieceData.sprite;
    }
}
