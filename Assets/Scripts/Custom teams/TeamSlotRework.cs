using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TeamSlotRework : MonoBehaviour
{
    public Image image;
    public GameData gameData;
    public int slotNum;
    public int mat = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnPointerClick()
    {
        TitleScreenButtons titleScreenButtons = TitleScreenButtons.Instance;
        DraggableUIPiece pieceData = titleScreenButtons.selectedPiece;
        SetPiece(pieceData.pieceId);
    }

    public void SetPiece(string pieceId)
    {
        TitleScreenButtons titleScreenButtons = TitleScreenButtons.Instance;
        if (!(titleScreenButtons.matValue - mat + PieceProperties.PieceValues[pieceId] > titleScreenButtons.maxMaterial))
            {
                if (titleScreenButtons.lifelineCount() > 0 && (pieceId == "StandardKing" || pieceId == "MpregKing"))
                {
                    titleScreenButtons.editMatTextStuff("Too many lifelines");
                }
                else
                {
                    titleScreenButtons.tempTeam[slotNum] = pieceId;
                    int currentMat = mat * -1;

                    titleScreenButtons.matValue -= mat;
                    mat = PieceProperties.PieceValues[pieceId];
                    titleScreenButtons.matValue += mat;
                    currentMat += mat;

                    image.sprite = UIPieceSpawner.Instance.NameToImage(pieceId);

                    titleScreenButtons.updateMatText();
                    titleScreenButtons.editMatTextNotError(pieceId + " " + ((currentMat >= 0) ? "+" + currentMat : currentMat.ToString()));
                }
                
            }
            else titleScreenButtons.editMatTextStuff("Max 39");
    }
}
