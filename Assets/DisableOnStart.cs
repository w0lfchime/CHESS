using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
