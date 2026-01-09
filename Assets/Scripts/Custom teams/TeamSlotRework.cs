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
    public int slotNum;
    public int mat = 0;
    public bool visual = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnPointerClick()
    {
        if(visual) return;
        TitleScreenButtons titleScreenButtons = TitleScreenButtons.Instance;
        DraggableUIPiece pieceData = titleScreenButtons.selectedPiece;

        if (image.sprite != PieceLibrary.Instance.NameToImage(""))
        {
            SetPiece("");
        }
        else
        {
            SetPiece(pieceData.pieceId.pieceName);
        }
    }

    public void SetPiece(string pieceId)
    {
        if (visual)
        {
            if(pieceId == "" || pieceId == null)
            {
                image.sprite = PieceLibrary.Instance.emptySprite;
                return;
            }
            image.sprite = PieceLibrary.Instance.NameToImage(pieceId);
            return;
        }
        TitleScreenButtons titleScreenButtons = TitleScreenButtons.Instance;
        if(pieceId == "" || pieceId == null)
        {
            image.sprite = PieceLibrary.Instance.NameToImage("");
            titleScreenButtons.tempTeam[slotNum] = null;
            titleScreenButtons.matValue -= mat;
            mat = 0;
            titleScreenButtons.updateMatText();
            return;
        }
        int materialValue = PieceLibrary.Instance.GetPrefab(pieceId).materialValue;
        if (!(titleScreenButtons.matValue - mat + materialValue > titleScreenButtons.maxMaterial))
            {
                if (titleScreenButtons.lifelineCount() > 0 && PieceLibrary.Instance.GetPrefab(pieceId).lifeLine)
                {
                    titleScreenButtons.editMatTextStuff("Too many lifelines");
                }
                else
                {
                    titleScreenButtons.tempTeam[slotNum] = pieceId;
                    int currentMat = mat * -1;

                    titleScreenButtons.matValue -= mat;
                    mat = materialValue;
                    titleScreenButtons.matValue += mat;
                    currentMat += mat;

                    image.sprite = PieceLibrary.Instance.NameToImage(pieceId);

                    titleScreenButtons.updateMatText();
                    titleScreenButtons.editMatTextNotError(pieceId + " " + ((currentMat >= 0) ? "+" + currentMat : currentMat.ToString()));
                }
                
            }
            else titleScreenButtons.editMatTextStuff("Max 39");
    }
}
