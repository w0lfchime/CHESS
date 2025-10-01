using UnityEngine;
using System.Collections;

public class TurnFlip : MonoBehaviour
{
    [Header("References")]
    public Chessboard chessboard;

    [Header("Flip Settings")]
    public float speed = 5f;
    public float delayTime = 1f;

    private bool isDelaying;
    private bool lastTurnWasBlack;

    void Update()
    {
        // Detect when turn changes
        bool blackTurn = false;
        if (lastTurnWasBlack != blackTurn)
        {
            lastTurnWasBlack = blackTurn;
            StartCoroutine(Delay()); // Start delay whenever turn changes
        }

        // Rotate only when not in delay
        if (!isDelaying)
        {
            Vector3 target = blackTurn ? new Vector3(0, 180, 0) : Vector3.zero;
            transform.eulerAngles = Vector3.Lerp(
                transform.eulerAngles, target, Time.deltaTime * speed
            );
        }
    }

    // Delays rotation for an amount of time
    private IEnumerator Delay()
    {
        isDelaying = true;
        yield return new WaitForSeconds(delayTime);
        isDelaying = false;
    }
}
