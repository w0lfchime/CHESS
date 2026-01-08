using UnityEngine;

public class HandMove : MonoBehaviour
{
    public Vector2 offset;
    public float turnAmount = 1;
    private Vector3 targetPos;
    void Start()
    {
        targetPos = transform.parent.position;
    }
    public void Move(Transform button)
    {
        targetPos = new Vector3(button.position.x+offset.x, button.position.y + offset.y, transform.parent.position.z);
    }
    
    void Update()
    {
        if(Vector2.Distance(targetPos, Camera.main.ScreenToViewportPoint(Input.mousePosition)) > 800)
        {
            transform.position += (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * .05f;
        }else
        {
            transform.position += (targetPos - transform.position) * .05f;
            transform.eulerAngles = new Vector3(0, 0, (targetPos.y - transform.position.y) * turnAmount);
        }
    }
}
