using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent doEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        doEvent.Invoke();
    }
}
