using UnityEngine;
using UnityEngine.EventSystems;

public class HandMove : MonoBehaviour
{
    public Vector2 offset;
    public float turnAmount = 1f;

    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;

    private Vector3 targetPos;
    public Transform JohnPawn;

    private Camera cam;
    public float zDepth;

    void Start()
    {
        cam = Camera.main;
        targetPos = transform.position;
    }

    public void Move(Transform button)
    {
        targetPos = new Vector3(
            button.position.x + offset.x,
            button.position.y + offset.y,
            transform.position.z
        );
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 johnPoseAdjusted = JohnPawn.position + Vector3.up * 3f;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth)
            );

            // Direction from John to mouse
            Vector3 dir = mouseWorld - johnPoseAdjusted;

            // HALF WAY toward mouse
            Vector3 halfwayTarget = johnPoseAdjusted + dir * 0.5f;

            // Smooth position
            transform.position = Vector3.Lerp(
                transform.position,
                halfwayTarget,
                Time.deltaTime * moveSpeed
            );

            // Rotate 90 degrees relative to direction
            Vector3 rotatedDir = Quaternion.Euler(0, 0, 90f) * dir.normalized;

            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, rotatedDir);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Move toward button target
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * moveSpeed
            );

            // Existing tilt logic, but smoothed
            float zTilt = (targetPos.y - transform.position.y) * turnAmount;
            Quaternion targetRot = Quaternion.Euler(0, 0, zTilt);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
