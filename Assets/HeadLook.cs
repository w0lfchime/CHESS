using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public float speed = 1;
    public float lessenLook = 10;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z-lessenLook;
        transform.forward += (transform.forward +(mousePos-transform.position)) * .001f * speed;
    }
}
