using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class pieceInfoDisplay : MonoBehaviour
{
    public static pieceInfoDisplay instance;
    void Start()
    {
        instance = this;
    }
    IEnumerator moveStep()
    {
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - new Vector3(500, 0, 0);
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForSeconds(0.01f);
            t += Time.deltaTime;
        }
    }

    public void display()
    {
        StartCoroutine(moveStep());
    }

}
