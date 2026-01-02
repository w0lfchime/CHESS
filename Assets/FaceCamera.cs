using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    void Update()
    {
        transform.eulerAngles = (Camera.main.transform.eulerAngles);
    }
}
