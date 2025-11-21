using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class pieceInfoDisplay : MonoBehaviour
{
    public static pieceInfoDisplay instance;
    public PieceInfoText pieceInfoText;
    public pieceInfoPicture pieceInfoPicture;
    public PieceInfoName pieceInfoName;
    public pieceInfoMoveset pieceInfoMoveset;
    public static bool inDisplay = false;
    private static bool isAnimating = false;
    public UnityEngine.Vector3 originalPosition;
    void Start()
    {
        instance = this;
        originalPosition = transform.position;
    }
    IEnumerator moveStep(float direction)
    {
        float t = 0;
        float factor = 35f;
        UnityEngine.Vector3 startPos = transform.position;
        UnityEngine.Vector3 endPos = startPos - new UnityEngine.Vector3(500, 0, 0) * direction;
        while (t < 1f)
        {
            transform.position = UnityEngine.Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForSeconds(0.01f);
            t += Time.deltaTime*factor;
            factor = Mathf.Max(Mathf.Pow(Mathf.Sqrt(factor) - 0.02f*Mathf.Sqrt(factor),2), 7f);
        }
        inDisplay = direction == 1 ? true : false;
        transform.position = endPos;
        isAnimating = false;
    }

    public void display(ChessPieceData pieceData)
    {
        Wrapper<Elements>[] vGrid = pieceData.abilities[0].actions[0].visualGrid;
        if (isAnimating) return;
        isAnimating = true;
        StartCoroutine(moveStep(1));
        pieceInfoText.disTex(pieceData.description);
        pieceInfoPicture.setPicture(pieceData.image);
        pieceInfoName.disTex(pieceData.pieceName);
        pieceInfoMoveset.disPic(vGrid);
    }
    public void unDisplay()
    {
        if (isAnimating) return;
        isAnimating = true;
        StartCoroutine(moveStep(-1));
    }

}
