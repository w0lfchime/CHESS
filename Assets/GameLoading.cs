using UnityEngine;
using PurrNet;

public class GameLoading : NetworkIdentity
{
    protected override void OnSpawned(bool asServer)
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }
}
