using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles = (Camera.main.transform.eulerAngles);
    }
}
